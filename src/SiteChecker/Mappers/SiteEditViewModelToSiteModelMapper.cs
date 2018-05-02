using SiteChecker.Helpers;
using SiteChecker.Models;
using SiteChecker.Models.ManageViewModels;

namespace SiteChecker.Mappers
{
	public class SiteEditViewModelToSiteModelMapper : IMapper<SiteEditViewModel, Sites>
	{
		public Sites Map(Sites result, SiteEditViewModel source)
		{
			result.CheckInterval = TimeHelper.GetInSeconds(source.IntervalHours, source.IntervalMinutes, source.IntervalSeconds);
			result.Url = source.Url;
			return result;
		}
	}
}