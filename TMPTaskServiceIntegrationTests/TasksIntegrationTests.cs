using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
			var newTask = new TaskRequestDTO { Name = TaskName, Description = TaskDescription };

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

        [Fact]
        public async Task FindTask_Should_Return_All_Appropriate_Tasks()
        {
            const string taskName = "Task";
            await CreateTask(taskName, null);
            await CreateTask(taskName, "SomeRandomDescription");
            await CreateTask($"Prefix {taskName}", "SomeOtherRandomDescription");
            await CreateTask("Another", null);
            var taskDTO = new TaskRequestDTO { Name = taskName};

            var response = await _client.GetFromJsonAsync<List<TaskReturnDTO>>($"/api/Task/FindTasks?{ToQueryString(taskDTO)}");

            response.Should().NotBeNull("FindTask endpoint should return list of tasks");
            response.Should().NotBeEmpty("FindTask endpoint should return filled list");
            response.Should().AllSatisfy(task =>
            {
                task.Name.Should().Contain(taskName, "All found tasks should contain given name");
           
            
            });
        }

        [Fact]
        public async Task FindTask_Should_Return_All_Appropriate_Tasks_With_Description()
        {
            const string taskName = "Task";
            const string taskDescription = "Other";
            await CreateTask(taskName, null);
            await CreateTask(taskName, "SomeRandomDescription");
            await CreateTask($"Prefix {taskName}", "SomeOtherRandomDescription");
            await CreateTask("Another", null);
            var taskDTO = new TaskRequestDTO { Name = taskName, Description = "Other"};

            var response = await _client.GetFromJsonAsync<List<TaskReturnDTO>>($"/api/Task/FindTasks?{ToQueryString(taskDTO)}");

            response.Should().NotBeNull("FindTask endpoint should return list of tasks");
            response.Should().NotBeEmpty("FindTask endpoint should return filled list");
            response.Should().AllSatisfy(task =>
            {
                task.Name.Should().Contain(taskName, "All found tasks should contain given name");
                task.Description.Should().Contain(taskDescription, "All found tasks should contain given description");
            });
        }

        [Fact]
        public async Task DeleteTask_Should_Delete_Task_With_Given_Id()
        {
            const string taskName = "Task";
            await CreateTask(taskName, null);
            await CreateTask(taskName, "SomeRandomDescription");
            await CreateTask($"Prefix {taskName}", "SomeOtherRandomDescription");
            await CreateTask("Another", null);
            var searchingData = new TaskRequestDTO { Name = taskName};
            var preDeleteSearchList = await _client.GetFromJsonAsync<List<TaskReturnDTO>>($"/api/Task/FindTasks?{ToQueryString(searchingData)}");
            var deletingTaskId = preDeleteSearchList!.First().Id;

            var result = await _client.DeleteAsync($"/api/Task/DeleteTask/{deletingTaskId}");

            var postDeleteSearchList = await _client.GetFromJsonAsync<List<TaskReturnDTO>>($"/api/Task/FindTasks?{ToQueryString(searchingData)}");
            postDeleteSearchList.Count.Should().Be(preDeleteSearchList.Count - 1, "We deleted 1 task");
            postDeleteSearchList.Should().NotContain(task => task.Id == deletingTaskId, "We deleted task with given id");
        }

		private async Task CreateTask(string taskName, string? taskDescription)
		{
			var newTask = new TaskRequestDTO { Name = taskName, Description = taskDescription };
            await _client.PostAsJsonAsync("/api/Task/CreateTask", newTask);
		}

		private static string ToQueryString(object obj)
        {
            if (obj == null) return string.Empty;

            var properties = from property in obj.GetType().GetProperties()
                             let value = property.GetValue(obj)
                             where value != null
                             select $"{Uri.EscapeDataString(property.Name)}={Uri.EscapeDataString(value.ToString()!)}";

            return string.Join("&", properties);
        }
    }
}
