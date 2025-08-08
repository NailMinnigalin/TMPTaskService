using TMPTaskService.Data.Interfaces;
using TMPTaskService.Domain.Interfaces;

namespace TMPTaskService.Domain.Implementations
{
	public class TaskManager(ITaskRepository taskRepository) : ITaskManager
	{
		private readonly ITaskRepository _taskRepository = taskRepository;

		public async Task CreateTaskAsync(string taskName, string? taskDescription)
		{
			await _taskRepository.SaveNewTaskAsync(new Data.Models.Task { Name = taskName, Description = taskDescription });
		}
	}
}
