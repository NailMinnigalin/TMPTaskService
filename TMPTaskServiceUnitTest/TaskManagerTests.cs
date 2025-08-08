using Microsoft.EntityFrameworkCore;
using TMPTaskService.Data.Repositories.Implementations;
using TMPTaskService.Domain.Implementations;
using TMPTaskService.Infrastructure;

namespace TMPTaskServiceUnitTest
{
	public class TaskManagerTests
	{
		public TaskManagerTests()
		{
			_options = new DbContextOptionsBuilder<TMPDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			using var context = new TMPDbContext(_options);
			context.Database.EnsureCreated();
		}

		private readonly DbContextOptions<TMPDbContext> _options;

		[Fact]
		public async Task CreateNewTask_SavesNewTask()
		{
			const string taskName = "Testing task";

			using(var context = new TMPDbContext(_options))
			{
				TaskManager taskManager = new(new DbTaskRepository(context));
				await taskManager.CreateTaskAsync(taskName, "TestDescription");
			}

			using(var context = new TMPDbContext(_options))
			{
				Assert.Equal(1, context.Tasks.Count());
				Assert.Equal(taskName, context.Tasks.Single().Name);
			}
		}

		[Fact]
		public async Task CreateNewTask_SavesNewTask_WithNullDescription()
		{
			const string taskName = "Testing task";

			using(var context = new TMPDbContext(_options))
			{
				TaskManager taskManager = new(new DbTaskRepository(context));
				await taskManager.CreateTaskAsync(taskName, null);
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
