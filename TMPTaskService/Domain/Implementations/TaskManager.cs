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

		public async Task DeleteTaskAsync(Guid taskId)
		{
			await _taskRepository.DeleteTaskAsync(taskId);
		}

		public async Task<List<Data.Models.Task>> FindTasksAsync(string taskName, string? taskDescription)
		{
			return await _taskRepository.FindTasksAsync(taskName, taskDescription);
		}
	}
}
