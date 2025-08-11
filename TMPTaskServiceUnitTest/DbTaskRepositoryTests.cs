using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TMPTaskService.Data.Implementations;
using TMPTaskService.Infrastructure;

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
			context.Tasks.Should().HaveCount(1);
			context.Tasks.Single().Name.Should().Be(taskName);
		}

		[Fact]
		public async Task FindTasks_ReturnAllMatchingTasks()
		{
			const string taskName = "Test task";
			await SaveNewTask(taskName, null);
			await SaveNewTask(taskName, null);
			await SaveNewTask(taskName, null);
			List<TMPTaskService.Data.Models.Task> tasks = await FindTask(taskName, null);

			tasks.Should().HaveCount(3);
		}

		[Fact]
		public async Task FindTasks_ReturnAllMatchingTasksConsideringDescription()
		{ 
			const string taskName = "Test task";
			const string taskDescription = "Description";
			await SaveNewTask(taskName, null);
			await SaveNewTask(taskName, taskDescription);
			await SaveNewTask(taskName, null);

			var tasks = await FindTask(taskName, taskDescription);

			tasks.Should().HaveCount(1);
			tasks.First().Description.Should().Be(taskDescription);
		}

		private async Task SaveNewTask(string taskName, string? taskDescription)
		{
			using var context = new TMPDbContext(_options);
			DbTaskRepository dbTaskRepository = new(context);
			TMPTaskService.Data.Models.Task task = new() { Name = taskName, Description = taskDescription };
			await dbTaskRepository.SaveNewTaskAsync(task);
		}

		private async Task<List<TMPTaskService.Data.Models.Task>> FindTask(string taskName, string? taskDescription)
		{
			using var context = new TMPDbContext(_options);
			DbTaskRepository dbTaskRepository = new(context);
			var tasks = await dbTaskRepository.FindTasksAsync(taskName, taskDescription);
			return tasks;
		}
	}
}
