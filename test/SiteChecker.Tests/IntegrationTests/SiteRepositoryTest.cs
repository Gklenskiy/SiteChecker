using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SiteChecker.Infrastructure;
using SiteChecker.Models;
using SiteChecker.Repositories;
using SiteChecker.Tests.Infrastructure;
using Xunit;

namespace SiteChecker.Tests.IntegrationTests
{
	public class SiteRepositoryTest
	{
		[Fact]
		public async Task GetAllAsync_ShouldReturnElements()
		{
			// Arrange
			var sites = new List<Sites>
			{
				new Sites
				{
					Id = 1
				},
				new Sites
				{
					Id = 2
				}
			};
			var db = new InMemoryDatabase();
			db.Insert(sites);
			var connectionFactoryMock = new Mock<IDatabaseConnectionFactory>();
			connectionFactoryMock.Setup(c => c.GetConnection()).Returns(db.OpenConnection());

			// Act
			var result = await new SitesRepository(connectionFactoryMock.Object).GetAllAsync();

			// Assert
			Assert.Equal(sites, result.ToList());
		}
	}
}