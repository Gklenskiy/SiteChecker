using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteChecker.Mappers;
using SiteChecker.Models;
using SiteChecker.Repositories;

namespace SiteChecker.Controllers
{
	[AllowAnonymous]
	public class HomeController : Controller
	{
		private readonly ISitesRepository _sitesRepository;
		private readonly IMapper<Sites, SiteViewModel> _mapper;

		public HomeController(ISitesRepository sitesRepository, IMapper<Sites, SiteViewModel> mapper)
		{
			_sitesRepository = sitesRepository;
			_mapper = mapper;
		}

		public async Task<IActionResult> Index()
		{
			var sites = await _sitesRepository.GetAllAsync();

			return View(sites.Select(x => _mapper.Map(new SiteViewModel(), x)).ToArray());
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
		}
	}
}