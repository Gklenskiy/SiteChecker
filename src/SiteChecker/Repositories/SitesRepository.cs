using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SiteChecker.Enums;
using SiteChecker.Infrastructure;
using SiteChecker.Models;

namespace SiteChecker.Repositories
{
	public class SitesRepository : ISitesRepository
	{
		private readonly IDatabaseConnectionFactory _connectionFactory;

		private const string TableName = "Sites";
		
		public SitesRepository(IDatabaseConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		private IDbConnection GetConnection()
		{
			return _connectionFactory.GetConnection();
		}

		public async Task AddAsync(Sites model)
		{
			using (var conn = GetConnection())
			{
				model.NextCheck = DateTime.Now.AddSeconds(model.CheckInterval);

				var query = $@"INSERT INTO {TableName} (Url, CheckInterval, NextCheck)
							 VALUES(@{nameof(Sites.Url)}, @{nameof(Sites.CheckInterval)}, @{nameof(Sites.NextCheck)})";
				conn.Open();
				await conn.ExecuteAsync(query, model);
			}
		}

		public async Task<Sites> GetSingleAsync(int id)
		{
			using (var conn = GetConnection())
			{
				var query = $"SELECT * FROM {TableName} where Id = @Id";

				conn.Open();
				var site = await conn.QuerySingleOrDefaultAsync<Sites>(query, new { Id = id });

				return site;
			}
		}

		public async Task<Sites[]> GetAllAsync()
		{
			using (var conn = GetConnection())
			{
				var query = $"SELECT * FROM {TableName}";

				conn.Open();
				var sites = await conn.QueryAsync<Sites>(query);

				return sites.ToArray();
			}
		}

		public async Task UpdateAsync(Sites model)
		{
			using (var conn = GetConnection())
			{
				var updQuery = $@"update {TableName} set CheckInterval = @{nameof(Sites.CheckInterval)}, NextCheck = DATETIME(LastCheck, '+{model.CheckInterval} seconds')
								where Id = @{nameof(Sites.Id)}";
				conn.Open();
				await conn.ExecuteAsync(updQuery, new
				{
					Id = model.Id,
					CheckInterval = model.CheckInterval
				});
			}
		}

		public async Task DeleteAsync(int id)
		{
			using (var conn = GetConnection())
			{
				var query = $"DELETE FROM {TableName} WHERE Id = @{nameof(Sites.Id)}";
				conn.Open();
				await conn.ExecuteAsync(query, new {Id = id});
			}
		}

		public async Task<Sites[]> GetSitesForCheckAsync()
		{
			using (var conn = GetConnection())
			{
				var query = $@"SELECT * 
							 FROM {TableName}
							 WHERE NextCheck <= @Now";

				conn.Open();
				var sites = await conn.QueryAsync<Sites>(query, new { Now = DateTime.Now });

				return sites.ToArray();
			}
		}

		public async Task UpdateStatusAsync(int id, SiteStatusCodes status)
		{
			using (var conn = GetConnection())
			{
				var updQuery = $@"UPDATE {TableName} 
								SET Status = @Status, LastCheck = @LastCheck, NextCheck = DATETIME(@LastCheck, '+' || CheckInterval || ' seconds') 
								WHERE Id = @Id";

				conn.Open();
				await conn.ExecuteAsync(updQuery, new
				{
					Id = id,
					Status = status.ToString(),
					LastCheck = DateTime.Now
				});
			}
		}
	}
}