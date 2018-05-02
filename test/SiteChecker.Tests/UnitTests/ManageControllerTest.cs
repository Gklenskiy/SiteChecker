using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SiteChecker.Controllers;
using SiteChecker.Mappers;
using SiteChecker.Models;
using SiteChecker.Models.ManageViewModels;
using SiteChecker.Repositories;
using Xunit;

namespace SiteChecker.Tests.UnitTests
{
	public class ManageControllerTest
	{
		[Fact]
		public async Task Index_ReturnsAViewResult_WithAListOfSites()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			var mapper = new SiteModelToSiteViewModelMapper();
			mockRepo.Setup(repo => repo.GetAllAsync()).Returns(Task.FromResult(GetTestSites()));
			var controller = new ManageController(mockRepo.Object, null, null, mapper);

			// Act
			var result = await controller.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<IEnumerable<SiteViewModel>>(
				viewResult.ViewData.Model);
			Assert.Equal(2, model.Count());
		}

		[Fact]
		public async Task CreatePost_ReturnsBadRequestResult_WhenModelStateIsInvalid()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			var controller = new ManageController(mockRepo.Object, null, null, null);
			controller.ModelState.AddModelError("Url", "Required");
			var newEditModel = new SiteEditViewModel();

			// Act
			var result = await controller.Create(newEditModel);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.IsType<SerializableError>(badRequestResult.Value);
		}

		[Theory]
		[InlineData(4)]
		[InlineData(86401)]
		public async Task CreatePost_ReturnsBadRequestResult_WhenIntervalOutOFRange(int checkInterval)
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			
			var mockMapper = new Mock<IMapper<SiteEditViewModel, Sites>>();
			mockMapper.Setup(mapper => mapper.Map(It.IsAny<Sites>(), It.IsAny<SiteEditViewModel>()))
				.Returns(new Sites
				{
					CheckInterval = checkInterval
				});
			
			var controller = new ManageController(mockRepo.Object, null, mockMapper.Object, null);
			var newEditModel = new SiteEditViewModel();

			// Act
			var result = await controller.Create(newEditModel);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task CreatePost_ReturnsARedirectAndAddsSite_WhenModelStateIsValid()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			var mapper = new SiteEditViewModelToSiteModelMapper();
			mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Sites>()))
				.Returns(Task.CompletedTask)
				.Verifiable();
			var controller = new ManageController(mockRepo.Object, null, mapper, null);
			var newEditModel = new SiteEditViewModel
			{
				Url = "Url",
				IntervalSeconds = 10,
			};

			// Act
			var result = await controller.Create(newEditModel);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Null(redirectToActionResult.ControllerName);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			mockRepo.Verify();
		}

		[Fact]
		public async Task Delete_ReturnsNotFoundResult_WhenIdIsNull()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			var controller = new ManageController(mockRepo.Object, null, null, null);

			// Act
			var result = await controller.Delete(null);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}
		
		[Fact]
		public async Task Delete_ReturnsNotFoundResult_WhenSiteNotFound()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			mockRepo.Setup(repo => repo.GetSingleAsync(It.IsAny<int>())).Returns(Task.FromResult<Sites>(null));
			var controller = new ManageController(mockRepo.Object, null, null, null);

			// Act
			var result = await controller.Delete(1);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}
		
		[Fact]
		public async Task Delete_ReturnsARedirectAndDeleteSite()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			mockRepo.Setup(repo => repo.GetSingleAsync(It.IsAny<int>())).Returns(Task.FromResult(new Sites()));
			mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<int>()))
				.Returns(Task.CompletedTask)
				.Verifiable();
			var controller = new ManageController(mockRepo.Object, null, null, null);

			// Act
			var result = await controller.Delete(1);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Null(redirectToActionResult.ControllerName);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			mockRepo.Verify();
		}
		
		[Fact]
		public async Task Edit_ReturnsBadRequestResult_WhenIdIsNull()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			var controller = new ManageController(mockRepo.Object, null, null, null);

			// Act
			var result = await controller.Edit(null);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}
		
		[Fact]
		public async Task Edit_ReturnsBadRequestResult_WhenSiteNotFound()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			mockRepo.Setup(repo => repo.GetSingleAsync(It.IsAny<int>())).Returns(Task.FromResult<Sites>(null));
			var controller = new ManageController(mockRepo.Object, null, null, null);

			// Act
			var result = await controller.Edit(1);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}
		
		[Fact]
		public async Task Edit_ReturnsARedirectAndViewResult()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			mockRepo.Setup(repo => repo.GetSingleAsync(It.IsAny<int>())).Returns(Task.FromResult(new Sites()));

			var mockMapper = new Mock<IMapper<Sites, SiteEditViewModel>>();
			mockMapper.Setup(mapper => mapper.Map(It.IsAny<SiteEditViewModel>(), It.IsAny<Sites>()))
				.Returns(new SiteEditViewModel());

			var controller = new ManageController(mockRepo.Object, mockMapper.Object, null, null);

			// Act
			var result = await controller.Edit(1);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.IsAssignableFrom<SiteEditViewModel>(viewResult.ViewData.Model);
		}

		[Fact]
		public async Task EditPost_ReturnsBadRequestResult_WhenModelStateIsInvalid()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			var controller = new ManageController(mockRepo.Object, null, null, null);
			controller.ModelState.AddModelError("Url", "Required");
			var editModel = new SiteEditViewModel();

			// Act
			var result = await controller.Edit(1, editModel);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.IsType<SerializableError>(badRequestResult.Value);
		}
		
		[Fact]
		public async Task EditPost_ReturnsNotFoundResult_WhenSiteNotFound()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			mockRepo.Setup(repo => repo.GetSingleAsync(It.IsAny<int>())).Returns(Task.FromResult<Sites>(null));
			var controller = new ManageController(mockRepo.Object, null, null, null);
			var editModel = new SiteEditViewModel();

			// Act
			var result = await controller.Edit(1, editModel);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Theory]
		[InlineData(4)]
		[InlineData(86401)]
		public async Task EditPost_ReturnsBadRequestResult_WhenIntervalOutOFRange(int checkInterval)
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			mockRepo.Setup(repo => repo.GetSingleAsync(It.IsAny<int>())).Returns(Task.FromResult(new Sites()));
			
			var mockMapper = new Mock<IMapper<SiteEditViewModel, Sites>>();
			mockMapper.Setup(mapper => mapper.Map(It.IsAny<Sites>(), It.IsAny<SiteEditViewModel>()))
				.Returns(new Sites
				{
					CheckInterval = checkInterval
				});

			var controller = new ManageController(mockRepo.Object, null, mockMapper.Object, null);
			var editModel = new SiteEditViewModel();

			// Act
			var result = await controller.Edit(1, editModel);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}
		
		[Fact]
		public async Task EditPost_ReturnsARedirectAndUpdateSite_WhenModelStateIsValid()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			mockRepo.Setup(repo => repo.GetSingleAsync(It.IsAny<int>())).Returns(Task.FromResult(new Sites()));
			mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Sites>()))
				.Returns(Task.CompletedTask)
				.Verifiable();
			
			var mockMapper = new Mock<IMapper<SiteEditViewModel, Sites>>();
			mockMapper.Setup(mapper => mapper.Map(It.IsAny<Sites>(), It.IsAny<SiteEditViewModel>()))
				.Returns(new Sites
				{
					CheckInterval = 100
				});
			
			var controller = new ManageController(mockRepo.Object, null, mockMapper.Object, null);
			var editModel = new SiteEditViewModel();

			// Act
			var result = await controller.Edit(1, editModel);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Null(redirectToActionResult.ControllerName);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			mockRepo.Verify();
		}

		private Sites[] GetTestSites()
		{
			var sessions = new List<Sites>
			{
				new Sites
				{
					Id = 1,
					Url = "Test Url"
				},
				new Sites
				{
					Id = 2,
					Url = "Test Url Two"
				}
			};
			return sessions.ToArray();
		}
	}
}