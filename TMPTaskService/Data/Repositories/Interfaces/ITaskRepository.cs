namespace TMPTaskService.Data.Repositories.Interfaces
{
	public interface ITaskRepository
	{
		public Task SaveTask(Models.Task task);
	}
}
