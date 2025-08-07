using Microsoft.EntityFrameworkCore;
using TMPTaskService.Data.Repositories.Implementations;
using TMPTaskService.Infrastructure;

namespace TMPTaskServiceUnitTest
{
	public class DbTaskRepositoryTests
	{
		[Fact]
		public async Task CreateTask_SaveTaskInDb()
		{
			var options = new DbContextOptionsBuilder<TMPDbContext>()
				.UseInMemoryDatabase(databaseName: "CreateTask_SaveTaskInDb")
				.Options;

			using(var context = new TMPDbContext(options))
			{
				DbTaskRepository dbTaskRepository = new(context);
				TMPTaskService.Data.Models.Task task = new() { Name = "Test task", Description = "Test description" };
				await dbTaskRepository.SaveTask(task);
			}

			using(var context = new TMPDbContext(options))
			{
				Assert.Equal(1, context.Tasks.Count());
				Assert.Equal("Test task", context.Tasks.Single().Name);
			}
		}
	}
}
