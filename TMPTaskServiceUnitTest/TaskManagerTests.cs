using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TMPTaskService.Data.Implementations;
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

			await CreateTask(taskName, "TestDescription");

			CheckTaskExists(taskName);
		}

		[Fact]
		public async Task CreateNewTask_SavesNewTask_WithNullDescription()
		{
			const string taskName = "Testing task";

			await CreateTask(taskName, null);

			CheckTaskExists(taskName);
		}

		public void Dispose()
		{
			using var context = new TMPDbContext(_options);
			context.Database.EnsureDeleted();
		}

		private void CheckTaskExists(string taskName)
		{
			using var context = new TMPDbContext(_options);
			context.Tasks.Should().HaveCount(1);
			context.Tasks.Single().Name.Should().Be(taskName);
		}

		private async Task CreateTask(string taskName, string? taskDescription)
		{
			using var context = new TMPDbContext(_options);
			TaskManager taskManager = new(new DbTaskRepository(context));
			await taskManager.CreateTaskAsync(taskName, taskDescription);
		}
	}
}
