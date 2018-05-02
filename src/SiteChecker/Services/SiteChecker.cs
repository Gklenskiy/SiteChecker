using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SiteChecker.Configuration;

namespace SiteChecker.Services
{
	public class SiteChecker : ISiteChecker
	{
		private readonly SitesBackgroundSettings _settings;
		
		public SiteChecker(IOptions<SitesBackgroundSettings> settings)
		{
			_settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
		}

		public async Task<bool> Check(string url)
		{
			using(var client = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(_settings.SiteTimeoutSeconds) 
			})
			{
				try
				{
					var result = await client.GetAsync(url);
					return result.StatusCode == HttpStatusCode.OK;
				}
				catch(TaskCanceledException e)
				{
					return false;
				}
				catch (HttpRequestException e)
				{
					return false;
				}
			}
		}
	}
}