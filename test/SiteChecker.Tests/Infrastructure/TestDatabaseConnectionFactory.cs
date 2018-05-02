using System.Data;
using SiteChecker.Infrastructure;

namespace SiteChecker.Tests.Infrastructure
{
	public class TestDatabaseConnectionFactory : IDatabaseConnectionFactory
	{
		private InMemoryDatabase _database;
		
		public TestDatabaseConnectionFactory()
		{
			_database = new InMemoryDatabase();
		}

		public IDbConnection GetConnection()
		{
			return _database.OpenConnection();
		}
	}
}