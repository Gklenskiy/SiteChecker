using System.Collections.Generic;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;

namespace SiteChecker.Tests.Infrastructure
{
	public class InMemoryDatabase
	{
		private readonly OrmLiteConnectionFactory _dbFactory = 
			new OrmLiteConnectionFactory(":memory:", SqliteOrmLiteDialectProvider.Instance);

		public IDbConnection OpenConnection() => _dbFactory.OpenDbConnection();

		public void Insert<T>(IEnumerable<T> items)
		{
			using (var db = OpenConnection())
			{
				db.CreateTableIfNotExists<T>();
				foreach (var item in items)
				{
					db.Insert(item);
				}
			}
		}
	}
}