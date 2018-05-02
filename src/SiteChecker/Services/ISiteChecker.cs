using System.Threading;
using System.Threading.Tasks;

namespace SiteChecker.Services
{
	public interface ISiteChecker
	{
		Task<bool> Check(string url);
	}
}