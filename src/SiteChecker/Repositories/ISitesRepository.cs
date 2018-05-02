using System.Threading.Tasks;
using SiteChecker.Enums;
using SiteChecker.Models;

namespace SiteChecker.Repositories
{
	public interface ISitesRepository
	{
		Task AddAsync(Sites model);

		Task DeleteAsync(int id);

		Task UpdateAsync(Sites model);

		Task<Sites> GetSingleAsync(int id);
		
		Task<Sites[]> GetAllAsync();

		Task<Sites[]> GetSitesForCheckAsync();

		Task UpdateStatusAsync(int id, int intervalSec, SiteStatusCodes status);
	}
}