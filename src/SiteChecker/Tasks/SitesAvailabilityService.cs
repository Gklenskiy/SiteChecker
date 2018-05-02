using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SiteChecker.Configuration;
using SiteChecker.Enums;
using SiteChecker.Models;
using SiteChecker.Repositories;
using SiteChecker.Services;

namespace SiteChecker.Tasks
{
	public class SitesAvailabilityService : ScopedBackgroundService
	{
		private readonly ISiteChecker _siteChecker;
		
		public SitesAvailabilityService(IOptions<SitesBackgroundSettings> settings, ILogger<SitesAvailabilityService> logger, IServiceScopeFactory serviceScopeFactory, ISiteChecker siteChecker) 
			: base(settings, logger, serviceScopeFactory)
		{
			_siteChecker = siteChecker ?? throw new ArgumentNullException(nameof(siteChecker));
		}

		protected override async Task ProcessInScope(IServiceProvider serviceProvider)
		{
			var siteRpository = serviceProvider.GetService<ISitesRepository>();
			await CheckSitesAvailability(siteRpository);
		}

		private async Task CheckSitesAvailability(ISitesRepository sitesRepository)
		{
			Logger.LogDebug($"Checking sites availability");

			var sites = await sitesRepository.GetSitesForCheckAsync();

			IEnumerable<Task<SiteCheckResult>> tasks = sites.Select(ProcessSite);

			List<Task<SiteCheckResult>> checkTasks = tasks.ToList();

			foreach(var bucket in TasksHelper.Interleaved(checkTasks)) { 
				var t = await bucket;
				try
				{
					var checkedSite = await t;
					await sitesRepository.UpdateStatusAsync(checkedSite.Id, checkedSite.CheckInterval,checkedSite.Status);
				}
				catch (OperationCanceledException)
				{

				}
				catch (SqlException exception)
				{
					Logger.LogCritical($"FATAL ERROR: Database connections could not be opened: {exception.Message}");
				}
				catch (Exception exc)
				{
					Logger.LogCritical($"FATAL ERROR: {exc.Message}");
				} 
			}
		}

		private async Task<SiteCheckResult> ProcessSite(Sites siteModel)
		{
			var checkResult = await _siteChecker.Check(siteModel.Url);
			return new SiteCheckResult
			{
				Id = siteModel.Id,
				CheckInterval = siteModel.CheckInterval,
				Status = checkResult ? SiteStatusCodes.Ok : SiteStatusCodes.Fail
			};
		}
	}
}