using Microsoft.EntityFrameworkCore;
using TMPTaskService.Data.Implementations;
using TMPTaskService.Infrastructure;

namespace TMPTaskServiceUnitTest
{
	public class DbTaskRepositoryTests
	{
		public DbTaskRepositoryTests()
		{
			_options = new DbContextOptionsBuilder<TMPDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			using var context = new TMPDbContext(_options);
			context.Database.EnsureCreated();
		}

		private readonly DbContextOptions<TMPDbContext> _options;

		[Fact]
		public async Task CreateTask_SaveTaskInDb()
		{
			const string taskName = "Test task";

			using(var context = new TMPDbContext(_options))
			{
				DbTaskRepository dbTaskRepository = new(context);
				
				TMPTaskService.Data.Models.Task task = new() { Name = taskName, Description = "Test description" };
				await dbTaskRepository.SaveNewTaskAsync(task);
			}

			using(var context = new TMPDbContext(_options))
			{
				Assert.Equal(1, context.Tasks.Count());
				Assert.Equal(taskName, context.Tasks.Single().Name);
			}
		}

		public void Dispose()
		{
			using var context = new TMPDbContext(_options);
			context.Database.EnsureDeleted();
		}
	}
}
