using SiteChecker.Enums;

namespace SiteChecker.Models
{
	public class SiteCheckResult
	{
		public int Id { get; set; }

		public SiteStatusCodes Status { get; set; }
	}
}