using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using TMPTaskService.Controllers;
using TMPTaskService.Infrastructure;

namespace TMPTaskServiceIntegrationTests
{
	public class TasksIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public TasksIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateTask_ShouldAddTaskToDatabase()
        {
			// Arrange
			const string TaskName = "Test Task";
			const string TaskDescription = "Integration Test";
			var newTask = new TaskDTO { Name = TaskName, Description = TaskDescription };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Task/CreateTask", newTask);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TMPDbContext>();

            var taskInDb = await db.Tasks.FirstOrDefaultAsync(t => t.Name == TaskName);
            taskInDb.Should().NotBeNull();
            taskInDb!.Description.Should().Be(TaskDescription);
        }
    }
}
