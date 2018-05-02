using SiteChecker.Models;

namespace SiteChecker.Mappers
{
	public class SiteModelToSiteViewModelMapper : IMapper<Sites, SiteViewModel>
	{
		public SiteViewModel Map(SiteViewModel result, Sites source)
		{
			result.Id = source.Id;
			result.Url = source.Url;
			result.Status = source.Status;
			result.LastCheck = source.LastCheck;
			
			return result;
		}
	}
}