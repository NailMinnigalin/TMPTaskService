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
		public async Task CreateNewTask_Saves_New_Task()
		{
			const string taskName = "Testing task";

			await CreateTask(taskName, "TestDescription");

			CheckTaskExists(taskName);
		}

		[Fact]
		public async Task CreateNewTask_Saves_New_Task_With_Null_Description()
		{
			const string taskName = "Testing task";

			await CreateTask(taskName, null);

			CheckTaskExists(taskName);
		}

		[Fact]
		public async Task FindTasks_Returns_All_Founded_Tasks()
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

			tasks.Should().HaveCount(3, $"We created 3 tasks with {taskName}");
		}

		[Fact]
		public async Task DeleteTask_Calls_ITaskRepository_DeleteTask_With_Given_Id()
		{
			Guid taskId = Guid.NewGuid();
			var mockTaskRepository = new Mock<ITaskRepository>();
			TaskManager taskManager = new(mockTaskRepository.Object);

			await taskManager.DeleteTaskAsync(taskId);

			mockTaskRepository.Verify(taskRepository => taskRepository.DeleteTaskAsync(taskId), Times.Once);
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
