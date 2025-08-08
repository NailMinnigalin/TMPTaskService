namespace TMPTaskService.Data.Interfaces
{
	public interface ITaskRepository
	{
		public Task SaveNewTaskAsync(Models.Task task);
	}
}
