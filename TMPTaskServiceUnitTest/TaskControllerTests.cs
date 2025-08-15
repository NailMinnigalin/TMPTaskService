using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TMPTaskService.Controllers;
using TMPTaskService.Data.Interfaces;
using TMPTaskService.Domain.Interfaces;

using Task = System.Threading.Tasks.Task;
using TMPTask = TMPTaskService.Data.Models.Task;

namespace UnitTests
{
	public class TaskControllerTests
	{
		[Fact]
		public async Task CreateTask_Return_Ok_When_Task_Created()
		{
			TaskController taskController = CreateTaskControllerForCreateTaskTesting();

			var result = await taskController.CreateTask(new TaskDTO() { Name = "TestTask", Description = "TestDescription" });

			result.Should().BeOfType<OkResult>();
		}

		[Fact]
		public async Task CreateTask_Return_Ok_When_Task_Created_With_Null_Description()
		{
			TaskController taskController = CreateTaskControllerForCreateTaskTesting();

			var result = await taskController.CreateTask(new TaskDTO() { Name = "TestTask" });

			result.Should().BeOfType<OkResult>();
		}

		[Fact]
		public async Task FindTasks_Returns_All_Tasks_That_Contains_Given_Name_And_Description()
		{
			const string taskName = "task";
			const string taskDescription = "description";
			TaskController taskController = CreateTaskControllerForFindTaskTesting(taskName, taskDescription);

			List<TaskDTO> result = await taskController.FindTasks(new TaskDTO() {  Name = taskName, Description = taskDescription });

			result.Should().AllSatisfy(t => t.Name.Contains(taskName), "All found tasks should contain given name");
			result.Should().AllSatisfy(t =>
			{
				t.Description.Should().NotBeNull("description should not be null");
				t.Description.Should().Contain(taskDescription, "all found tasks should contain given description");
			});
		}

		[Fact]
		public async Task FindTasks_Returns_All_Tasks_That_Contains_Given_Name_And_Description_With_Null_Description()
		{
			const string taskName = "task";
			const string? taskDescription = null;
			TaskController taskController = CreateTaskControllerForFindTaskTesting(taskName, taskDescription);

			List<TaskDTO> result = await taskController.FindTasks(new TaskDTO() { Name = taskName, Description = taskDescription });

			result.Should().AllSatisfy(t => t.Name.Contains(taskName), "All found tasks should contain given name");
		}

		[Fact]
		public async Task DeleteTask_Calls_ITaskManager_DeleteTask_With_Given_Id()
		{
			Guid taskId = Guid.NewGuid();
			var mockTaskManager = new Mock<ITaskManager>();
			TaskController taskController = new(mockTaskManager.Object);

			await taskController.DeleteTask(taskId);

			mockTaskManager.Verify(taskManager => taskManager.DeleteTaskAsync(taskId), Times.Once());
		}

		private static TaskController CreateTaskControllerForCreateTaskTesting()
		{
			var mockTaskManager = new Mock<ITaskManager>();
			mockTaskManager.Setup(m => m.CreateTaskAsync(It.IsAny<string>(), It.IsAny<string>()));
			TaskController taskController = new(mockTaskManager.Object);
			return taskController;
		}

		private static TaskController CreateTaskControllerForFindTaskTesting(string taskName, string? taskDescription)
		{
			List<TMPTask> tasks = new();
			if (taskDescription != null)
			{
				tasks.Add(new() { Name = taskName, Description = taskDescription });
				tasks.Add(new() { Name = $"SomeRandomNamePrefix {taskName} SomeRandomNamePostfix", Description = $"SomeRandomDescriptionPrefix {taskDescription} SomeRandomDescriptionPostFix" });
			}
			else
			{
				tasks.Add(new() { Name = taskName, Description = taskDescription });
				tasks.Add(new() { Name = $"SomeRandomNamePrefix SomeRandomNamePostfix", Description = $"SomeRandomDescriptionPrefix SomeRandomDescriptionPostFix" });
				tasks.Add(new() { Name = taskName, Description = null});
			}

			var mockTaskManager = new Mock<ITaskManager>();
			mockTaskManager.Setup(m => m.FindTasksAsync(It.Is<string>(s => s == taskName), It.Is<string>(s => s == taskDescription))).ReturnsAsync(tasks);
			TaskController taskController = new(mockTaskManager.Object);
			return taskController;
		}
	}
}
