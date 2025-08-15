using Microsoft.AspNetCore.Mvc;
using TMPTaskService.Domain.Interfaces;

namespace TMPTaskService.Controllers
{
	public class TaskReturnDTO
	{
		public required Guid Id { get; init; }
		public required string Name { get; init; }
		public string? Description { get; init; }
	}

	public class TaskRequestDTO
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
		public async Task<IActionResult> CreateTask([FromBody] TaskRequestDTO createTaskRequest)
		{
			await _taskManager.CreateTaskAsync(createTaskRequest.Name, createTaskRequest.Description);
			return Ok();
		}

		[HttpGet("FindTasks")]
		public async Task<List<TaskReturnDTO>> FindTasks([FromQuery] TaskRequestDTO findTasksRequest)
		{
			var result = await _taskManager.FindTasksAsync(findTasksRequest.Name, findTasksRequest.Description);
			return result.Select(t => new TaskReturnDTO() { Id = t.Id, Name = t.Name, Description = t.Description}).ToList();
		}

		[HttpDelete("DeleteTask/{taskId}")]
		public async Task<IActionResult> DeleteTask(Guid taskId)
		{
			await _taskManager.DeleteTaskAsync(taskId);
			return Ok();
		}
	}
}
