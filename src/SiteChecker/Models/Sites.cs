using System;

namespace SiteChecker.Models
{
	public class Sites
	{
		public int Id { get; set; }

		public string Url { get; set; }

		public string Status { get; set; }

		public int CheckInterval { get; set; }

		public DateTime LastCheck { get; set; }

		public DateTime NextCheck { get; set; }
	}
}