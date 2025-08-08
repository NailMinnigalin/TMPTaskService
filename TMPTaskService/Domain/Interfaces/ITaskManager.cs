namespace TMPTaskService.Domain.Interfaces
{
	public interface ITaskManager
	{
		public Task CreateTaskAsync(string taskName, string? taskDescription);
	}
}
