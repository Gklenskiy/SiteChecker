namespace SiteChecker.Helpers
{
	public static class TimeHelper
	{
		public static int GetInSeconds(int hours, int minutes, int seconds)
		{
			return hours * 60 * 60 + minutes * 60 + seconds;
		}
		
		public static int GetWholeHours(int seconds)
		{
			return seconds / 3600;
		}
		
		public static int GetWholeMinutes(int seconds)
		{
			return (seconds % 3600) / 60;
		}
		
		public static int GetWholeSeconds(int seconds)
		{
			return (seconds % 3600) % 60;
		}
	}
}