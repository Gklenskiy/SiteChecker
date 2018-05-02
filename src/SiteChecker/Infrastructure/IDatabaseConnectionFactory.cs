using System.Data;

namespace SiteChecker.Infrastructure
{
	public interface IDatabaseConnectionFactory
	{
		IDbConnection GetConnection();
	}
}