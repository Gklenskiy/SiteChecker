using System.ComponentModel.DataAnnotations;

namespace SiteChecker.Models.ManageViewModels
{
	public class SiteEditViewModel
	{
		public int SiteId { get; set; }
		
		[Required]
		[DataType(DataType.Url)]
		public string Url { get; set; }

		[Range(0, 24)]
		[Display(Name = "Check interval hours")]
		public int IntervalHours{ get; set; }

		[Range(0, 59)]
		[Display(Name = "Check interval minutes")]
		public int IntervalMinutes{ get; set; }

		[Range(0, 59)]
		[Display(Name = "Check interval seconds")]
		public int IntervalSeconds{ get; set; }
	}
}