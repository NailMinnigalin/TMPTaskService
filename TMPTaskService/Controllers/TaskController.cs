using Microsoft.AspNetCore.Mvc;
using TMPTaskService.Domain.Interfaces;

namespace TMPTaskService.Controllers
{
	public class CreateTaskRequest
	{
		public required string Name { get; init; }
		public string? Description { get; init; }
	}

	[ApiController]
	[Route("api/[controller]")]
	public class TaskController(ITaskManager taskManager) : Controller
	{
		private readonly ITaskManager _taskManager = taskManager;

		[HttpPost("CreateTask")]
		public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest createTaskRequest)
		{
			await _taskManager.CreateTaskAsync(createTaskRequest.Name, createTaskRequest.Description);
			return Ok();
		}
	}
}
