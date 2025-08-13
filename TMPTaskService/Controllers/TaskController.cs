using Microsoft.AspNetCore.Mvc;
using TMPTaskService.Domain.Interfaces;

namespace TMPTaskService.Controllers
{
	public class TaskDTO
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
		public async Task<IActionResult> CreateTask([FromBody] TaskDTO createTaskRequest)
		{
			await _taskManager.CreateTaskAsync(createTaskRequest.Name, createTaskRequest.Description);
			return Ok();
		}

		[HttpGet("FindTasks")]
		public async Task<List<TaskDTO>> FindTasks(TaskDTO findTasksRequest)
		{
			var result = await _taskManager.FindTasksAsync(findTasksRequest.Name, findTasksRequest.Description);
			return result.Select(t => new TaskDTO() { Name = t.Name, Description = t.Description}).ToList();
		}
	}
}
