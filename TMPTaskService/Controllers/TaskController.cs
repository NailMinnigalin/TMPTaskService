using Microsoft.AspNetCore.Mvc;
using TMPTaskService.Domain.Interfaces;

namespace TMPTaskService.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TaskController : Controller
	{
		public TaskController(ITaskManager taskManager)
		{
			_taskManager = taskManager;
		}

		private readonly ITaskManager _taskManager;

		[HttpPost]
		public async Task<IActionResult> CreateTask(string name, string? description)
		{
			await _taskManager.CreateTaskAsync(name, description);
			return Ok();
		}
	}
}
