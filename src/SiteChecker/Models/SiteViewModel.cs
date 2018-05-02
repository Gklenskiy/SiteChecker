using System;

namespace SiteChecker.Models
{
	public class SiteViewModel
	{
		public int Id { get; set; }

		public string Url { get; set; }

		public string Status { get; set; }

		public DateTime LastCheck { get; set; }
	}
}