namespace TMPTaskService.Data.Repositories.Interfaces
{
	public interface ITaskRepository
	{
		public Task SaveNewTaskAsync(Models.Task task);
	}
}
