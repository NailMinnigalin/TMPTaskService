namespace TMPTaskService.Data.Interfaces
{
	public interface ITaskRepository
	{
		public Task SaveNewTaskAsync(Models.Task task);
		public Task<List<Models.Task>> FindTasksAsync(string name, string? description);
	}
}
