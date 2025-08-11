using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TMPTaskService.Controllers;
using TMPTaskService.Domain.Interfaces;

namespace UnitTests
{
	public class TaskControllerTests
	{
		[Fact]
		public async Task CreateTask_Return_Ok_When_Task_Created()
		{
			TaskController taskController = CreateTaskController();

			var result = await taskController.CreateTask(new CreateTaskRequest() { Name = "TestTask", Description = "TestDescription" });

			result.Should().BeOfType<OkResult>();
		}

		[Fact]
		public async Task CreateTask_Return_Ok_When_Task_Created_With_Null_Description()
		{
			TaskController taskController = CreateTaskController();

			var result = await taskController.CreateTask(new CreateTaskRequest() { Name = "TestTask" });

			result.Should().BeOfType<OkResult>();
		}

		private static TaskController CreateTaskController()
		{
			var mockTaskManager = new Mock<ITaskManager>();
			mockTaskManager.Setup(m => m.CreateTaskAsync(It.IsAny<string>(), It.IsAny<string>()));
			TaskController taskController = new(mockTaskManager.Object);
			return taskController;
		}
	}
}
