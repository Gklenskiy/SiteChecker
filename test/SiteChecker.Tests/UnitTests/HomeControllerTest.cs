using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SiteChecker.Controllers;
using SiteChecker.Mappers;
using SiteChecker.Models;
using SiteChecker.Repositories;
using Xunit;

namespace SiteChecker.Tests.UnitTests
{
	public class HomeControllerTest
	{
		[Fact]
		public async Task Index_ReturnsAViewResult_WithAListOfSites()
		{
			// Arrange
			var mockRepo = new Mock<ISitesRepository>();
			var mapper = new SiteModelToSiteViewModelMapper();
			mockRepo.Setup(repo => repo.GetAllAsync()).Returns(Task.FromResult(GetTestSites()));
			var controller = new HomeController(mockRepo.Object, mapper);

			// Act
			var result = await controller.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<IEnumerable<SiteViewModel>>(
				viewResult.ViewData.Model);
			Assert.Equal(2, model.Count());
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