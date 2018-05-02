using SiteChecker.Helpers;
using SiteChecker.Models;
using SiteChecker.Models.ManageViewModels;

namespace SiteChecker.Mappers
{
	public class SiteModelToSiteEditViewModelMapper : IMapper<Sites, SiteEditViewModel>
	{
		public SiteEditViewModel Map(SiteEditViewModel result, Sites source)
		{
			var hours = TimeHelper.GetWholeHours(source.CheckInterval);
			var minutes = TimeHelper.GetWholeMinutes(source.CheckInterval);
			var sec = TimeHelper.GetWholeSeconds(source.CheckInterval);

			result.Url = source.Url;
			result.IntervalHours = hours;
			result.IntervalMinutes = minutes;
			result.IntervalSeconds = sec;

			return result;
		}
	}
}