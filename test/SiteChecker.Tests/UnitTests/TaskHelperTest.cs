using System.Collections.Generic;
using System.Threading.Tasks;
using SiteChecker.Tasks;
using Xunit;

namespace SiteChecker.Tests.UnitTests
{
	public class TaskHelperTest
	{
		[Fact]
		public async Task Interleaved_ShouldExecuteTasksByDuration()
		{
			var tasks = new[]
			{
				Task.Delay(3000).ContinueWith(_ => 3),
				Task.Delay(1000).ContinueWith(_ => 1),
				Task.Delay(2000).ContinueWith(_ => 2),
				Task.Delay(5000).ContinueWith(_ => 5),
				Task.Delay(4000).ContinueWith(_ => 4),
			};

			var res = new List<int>();
			foreach (var bucket in TasksHelper.Interleaved(tasks)) { 
				var t = await bucket; 
				var result = await t;
				res.Add(result);
			}
			
			Assert.Collection(res,
				item => Assert.Equal(1, item),
				item => Assert.Equal(2, item),
				item => Assert.Equal(3, item),
				item => Assert.Equal(4, item),
				item => Assert.Equal(5, item)
			);
		}
	}
}