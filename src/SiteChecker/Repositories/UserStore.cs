using System.Data;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SiteChecker.Models;

namespace SiteChecker.Repositories
{
	public class UserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>,
		IUserEmailStore<ApplicationUser>
	{
		private readonly string _connectionString;

		public UserStore(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("Users");
		}

		public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
		{
			user.Email = email;
			return Task.FromResult(0);
		}

		public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Email);
		}

		public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.EmailConfirmed);
		}

		public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
		{
			user.EmailConfirmed = confirmed;
			return Task.FromResult(0);
		}

		public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
		{
			using (var connection = GetConnection())
			{
				connection.Open();
				return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM ApplicationUser
                    WHERE NormalizedEmail = @{nameof(normalizedEmail)}", new {normalizedEmail});
			}
		}

		public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.NormalizedEmail);
		}

		public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
		{
			user.NormalizedEmail = normalizedEmail;
			return Task.FromResult(0);
		}

		public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
		{
			user.PasswordHash = passwordHash;
			return Task.FromResult(0);
		}

		public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.PasswordHash != null);
		}

		public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			using (var connection = GetConnection())
			{
				connection.Open();
				user.Id = await connection.QuerySingleAsync<int>($@"INSERT INTO ApplicationUser (UserName, NormalizedUserName, Email,
                    NormalizedEmail, EmailConfirmed, PasswordHash)
                    VALUES (@{nameof(ApplicationUser.UserName)}, @{nameof(ApplicationUser.NormalizedUserName)}, @{nameof(ApplicationUser.Email)},
                    @{nameof(ApplicationUser.NormalizedEmail)}, @{nameof(ApplicationUser.EmailConfirmed)}, @{nameof(ApplicationUser.PasswordHash)});
                    SELECT last_insert_rowid();", user);
			}

			return IdentityResult.Success;
		}

		public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			using (var connection = GetConnection())
			{
				connection.Open();
				await connection.ExecuteAsync($"DELETE FROM ApplicationUser WHERE Id = @{nameof(ApplicationUser.Id)}", user);
			}

			return IdentityResult.Success;
		}

		public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			using (var connection = GetConnection())
			{
				connection.Open();
				return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM ApplicationUser
                    WHERE Id = @{nameof(userId)}", new {userId});
			}
		}

		public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			using (var connection = GetConnection())
			{
				connection.Open();
				return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM ApplicationUser
                    WHERE NormalizedUserName = @{nameof(normalizedUserName)}", new {normalizedUserName});
			}
		}

		public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.NormalizedUserName);
		}

		public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Id.ToString());
		}

		public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.UserName);
		}

		public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName,
			CancellationToken cancellationToken)
		{
			user.NormalizedUserName = normalizedName;
			return Task.FromResult(0);
		}

		public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
		{
			user.UserName = userName;
			return Task.FromResult(0);
		}

		public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			using (var connection = GetConnection())
			{
				connection.Open();
				await connection.ExecuteAsync($@"UPDATE ApplicationUser SET
                    UserName = @{nameof(ApplicationUser.UserName)},
                    NormalizedUserName = @{nameof(ApplicationUser.NormalizedUserName)},
                    Email = @{nameof(ApplicationUser.Email)},
                    NormalizedEmail = @{nameof(ApplicationUser.NormalizedEmail)},
                    EmailConfirmed = @{nameof(ApplicationUser.EmailConfirmed)},
                    PasswordHash = @{nameof(ApplicationUser.PasswordHash)},
                    WHERE Id = @{nameof(ApplicationUser.Id)}", user);
			}

			return IdentityResult.Success;
		}

		public void Dispose()
		{
			// Nothing to dispose.
		}

		public IDbConnection GetConnection()
		{
			return new SQLiteConnection(_connectionString);
		}
	}
}