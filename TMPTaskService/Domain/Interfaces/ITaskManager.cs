using Task = System.Threading.Tasks.Task;

namespace TMPTaskService.Domain.Interfaces
{
	public interface ITaskManager
	{
		public Task CreateTaskAsync(string taskName, string? taskDescription);
		public Task DeleteTaskAsync(Guid taskId);
		public Task<List<Data.Models.Task>> FindTasksAsync(string taskName, string? taskDescription);
	}
}
