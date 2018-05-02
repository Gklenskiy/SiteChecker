using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SiteChecker.Configuration;

namespace SiteChecker.Tasks
{
	public abstract class ScopedBackgroundService : BackgroundService
	{
		private readonly SitesBackgroundSettings _settings;
		protected readonly ILogger<ScopedBackgroundService> Logger;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public ScopedBackgroundService(IOptions<SitesBackgroundSettings> settings,
			ILogger<ScopedBackgroundService> logger, IServiceScopeFactory serviceScopeFactory)
		{
			_settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
			
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Logger.LogDebug($"BackgroundService is starting.");

			stoppingToken.Register(() => 
				Logger.LogDebug($"Background task is stopping."));

			while (!stoppingToken.IsCancellationRequested)
			{
				Logger.LogDebug($"Task doing background work.");

				var processTask = Process();
				var delayTask = Task.Delay(_settings.CheckUpdateTime, stoppingToken);

				await processTask;
				await delayTask;
			}

			Logger.LogDebug($"Background task is stopping.");
		}

		private async Task Process()
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				await ProcessInScope(scope.ServiceProvider);
			}
		}

		protected abstract Task ProcessInScope(IServiceProvider serviceProvider);

	}
}