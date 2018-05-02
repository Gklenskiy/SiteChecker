using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteChecker.Mappers;
using SiteChecker.Models;
using SiteChecker.Models.ManageViewModels;
using SiteChecker.Repositories;

namespace SiteChecker.Controllers
{
	[Authorize]
	public class ManageController : Controller
	{
		private readonly ISitesRepository _sitesRepository;
		private readonly IMapper<Sites, SiteEditViewModel> _modelToEditViewModelMapper;
		private readonly IMapper<SiteEditViewModel, Sites> _editModelToModelMapper;
		private readonly IMapper<Sites, SiteViewModel> _modelToViewModelMapper;
		private const int MaxCheckTimeSeconds = 86400; // 24 hours
		private const int MinCheckTimeSeconds = 5;
		
		public ManageController(ISitesRepository sitesRepository,
			IMapper<Sites, SiteEditViewModel> modelToEditViewModelMapper,
			IMapper<SiteEditViewModel, Sites> editModelToModelMapper,
			IMapper<Sites, SiteViewModel> modelToViewModelMapper)
		{
			_sitesRepository = sitesRepository;
			_modelToEditViewModelMapper = modelToEditViewModelMapper;
			_editModelToModelMapper = editModelToModelMapper;
			_modelToViewModelMapper = modelToViewModelMapper;
		}

		// GET
		public async Task<IActionResult> Index()
		{
			var sites = await _sitesRepository.GetAllAsync();

			return View(sites.Select(x => _modelToViewModelMapper.Map(new SiteViewModel(), x)).ToArray());
		}
		
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var site = await _sitesRepository.GetSingleAsync(id.Value);
			if (site == null)
			{
				return NotFound();
			}

			var editModel = _modelToEditViewModelMapper.Map(new SiteEditViewModel(), site);
			return View(editModel);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, SiteEditViewModel editModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			
			var site = await _sitesRepository.GetSingleAsync(id);
			if (site == null)
			{
				return NotFound();
			}

			var siteModel = _editModelToModelMapper.Map(site, editModel);
			if (siteModel.CheckInterval < MinCheckTimeSeconds || siteModel.CheckInterval > MaxCheckTimeSeconds)
			{
				return BadRequest();
			}
			
			await _sitesRepository.UpdateAsync(siteModel);
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var site = await _sitesRepository.GetSingleAsync(id.Value);
			if (site == null)
			{
				return NotFound();
			}
			
			await _sitesRepository.DeleteAsync(id.Value);

			return RedirectToAction("Index");
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(SiteEditViewModel editModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var siteModel = _editModelToModelMapper.Map(new Sites(), editModel);
			if (siteModel.CheckInterval < MinCheckTimeSeconds || siteModel.CheckInterval > MaxCheckTimeSeconds)
			{
				return BadRequest();
			}

			await _sitesRepository.AddAsync(siteModel);
			
			return RedirectToAction(nameof(Index));
		}
	}
}