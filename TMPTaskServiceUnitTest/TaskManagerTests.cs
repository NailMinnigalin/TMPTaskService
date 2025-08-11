using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TMPTaskService.Data.Implementations;
using TMPTaskService.Data.Interfaces;
using TMPTaskService.Domain.Implementations;
using TMPTaskService.Infrastructure;
using Task = System.Threading.Tasks.Task;

namespace UnitTests
{
	public class TaskManagerTests
	{
		private readonly DbContextOptions<TMPDbContext> _options;

		public TaskManagerTests()
		{
			_options = new DbContextOptionsBuilder<TMPDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			using var context = new TMPDbContext(_options);
			context.Database.EnsureCreated();
		}

		public void Dispose()
		{
			using var context = new TMPDbContext(_options);
			context.Database.EnsureDeleted();
		}

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

		[Fact]
		public async Task FindTasks_ReturnAllFoundedTasks()
		{
			const string taskName = "Testing task";
			var mockTaskRepository = new Mock<ITaskRepository>();
			mockTaskRepository.Setup(m => m.FindTasksAsync(It.Is<string>(s => s == taskName), It.IsAny<string>())).ReturnsAsync(
			[
				new() { Name = taskName },
				new() { Name = taskName },
				new() { Name = taskName }
			]);
			TaskManager taskManager = new(mockTaskRepository.Object);

			var tasks = await taskManager.FindTasksAsync(taskName, null);

			tasks.Should().HaveCount(3);
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
