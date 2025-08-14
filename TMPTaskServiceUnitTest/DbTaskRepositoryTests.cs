using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TMPTaskService.Data.Implementations;
using TMPTaskService.Infrastructure;

using TMPTask = TMPTaskService.Data.Models.Task;

namespace UnitTests
{
	public class DbTaskRepositoryTests
	{
		private readonly DbContextOptions<TMPDbContext> _options;

		public DbTaskRepositoryTests()
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
		public async Task CreateTask_SaveTaskInDb()
		{
			const string taskName = "Test task";

			await SaveNewTask(taskName, null);

			using var context = new TMPDbContext(_options);
			context.Tasks.Should().HaveCount(1, "We created one task");
			context.Tasks.Single().Name.Should().Be(taskName, $"We created task with name {taskName}");
		}

		[Fact]
		public async Task FindTasks_ReturnAllMatchingTasks()
		{
			const string taskName = "Test task";
			await SaveNewTask(taskName, null);
			await SaveNewTask(taskName, null);
			await SaveNewTask(taskName, null);
			List<TMPTask> tasks = await FindTaskAsync(taskName, null);

			tasks.Should().HaveCount(3, $"We created three tasks with {taskName}");
		}

		[Fact]
		public async Task FindTasks_ReturnAllMatchingTasksConsideringDescription()
		{ 
			const string taskName = "Test task";
			const string taskDescription = "Description";
			await SaveNewTask(taskName, null);
			await SaveNewTask(taskName, taskDescription);
			await SaveNewTask(taskName, null);

			var tasks = await FindTaskAsync(taskName, taskDescription);

			tasks.Should().HaveCount(1, $"We created only one task with {taskName} and {taskDescription}");
			tasks.First().Description.Should().Contain(taskDescription);
		}

		[Fact]
		public async Task DeleteTask_DeleteTaskWithGivenId()
		{
			const string taskName = "Test task";
			await SaveNewTask(taskName, null);
			List<TMPTask> tasks = await FindTaskAsync(taskName, null);

			await DeleteTask(tasks);

			tasks = await FindTaskAsync(taskName, null);
			tasks.Should().BeEmpty("Task should be deleted");
		}

		private async Task DeleteTask(List<TMPTask> tasks)
		{
			var context = new TMPDbContext(_options);
			DbTaskRepository dbTaskRepository = new(context);
			await dbTaskRepository.DeleteTaskAsync(tasks.First().Id);
		}

		private async Task SaveNewTask(string taskName, string? taskDescription)
		{
			using var context = new TMPDbContext(_options);
			DbTaskRepository dbTaskRepository = new(context);
			TMPTask task = new() { Name = taskName, Description = taskDescription };
			await dbTaskRepository.SaveNewTaskAsync(task);
		}

		private async Task<List<TMPTask>> FindTaskAsync(string taskName, string? taskDescription)
		{
			using var context = new TMPDbContext(_options);
			DbTaskRepository dbTaskRepository = new(context);
			var tasks = await dbTaskRepository.FindTasksAsync(taskName, taskDescription);
			return tasks;
		}
	}
}
