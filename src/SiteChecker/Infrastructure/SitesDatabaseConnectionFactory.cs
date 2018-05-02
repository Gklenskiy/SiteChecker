using System.Data;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;

namespace SiteChecker.Infrastructure
{
	public class SitesDatabaseConnectionFactory : IDatabaseConnectionFactory
	{
		private readonly string _connectionString;
		
		public SitesDatabaseConnectionFactory(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("Default");
		}

		public IDbConnection GetConnection()
		{
			return new SQLiteConnection(_connectionString);
		}
	}
}