using Microsoft.AspNetCore.Mvc;
using Moq;
using TMPTaskService.Controllers;
using TMPTaskService.Domain.Interfaces;

namespace TMPTaskServiceUnitTest
{
	public class TaskControllerTests
	{
		[Fact]
		public async Task CreateTask_Return_Ok_When_Task_Created()
		{
			var mockTaskManager = new Mock<ITaskManager>();
			mockTaskManager.Setup(m => m.CreateTaskAsync(It.IsAny<string>(), It.IsAny<string>()));
			TaskController taskController = new(mockTaskManager.Object);

			var result = await taskController.CreateTask("TestTask", "TestDescription");

			Assert.IsType<OkResult>(result);
		}

		[Fact]
		public async Task CreateTask_Return_Ok_When_Task_Created_With_Null_Description()
		{
			var mockTaskManager = new Mock<ITaskManager>();
			mockTaskManager.Setup(m => m.CreateTaskAsync(It.IsAny<string>(), It.IsAny<string>()));
			TaskController taskController = new(mockTaskManager.Object);

			var result = await taskController.CreateTask("TestTask", null);

			Assert.IsType<OkResult>(result);
		}
	}
}
