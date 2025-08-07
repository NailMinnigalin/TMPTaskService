using TMPTaskService.Data.Repositories.Interfaces;
using TMPTaskService.Infrastructure;

namespace TMPTaskService.Data.Repositories.Implementations
{
	public class DbTaskRepository(TMPDbContext dbContext) : ITaskRepository
	{
		private readonly TMPDbContext _dbContext = dbContext;

		public async Task SaveTask(Models.Task task)
		{
			_dbContext.Tasks.Add(task);
			await _dbContext.SaveChangesAsync();
		}
	}
}
