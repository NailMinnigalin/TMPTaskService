using Microsoft.EntityFrameworkCore;
using TMPTaskService.Data.Interfaces;
using TMPTaskService.Infrastructure;

namespace TMPTaskService.Data.Implementations
{
	public class DbTaskRepository(TMPDbContext dbContext) : ITaskRepository
	{
		private readonly TMPDbContext _dbContext = dbContext;

		public async Task DeleteTaskAsync(Guid id)
		{
			var task = _dbContext.Tasks.Where(t => t.Id == id).FirstOrDefault();
			if (task != null)
			{
				_dbContext.Tasks.Remove(task);
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task<List<Models.Task>> FindTasksAsync(string name, string? description)
		{
			var tasks = _dbContext.Tasks.Where(t => t.Name.Contains(name));
			if (description != null) tasks = tasks.Where(t => t.Description != null && t.Description.Contains(description));

			return await tasks.ToListAsync();
		}

		public async Task SaveNewTaskAsync(Models.Task task)
		{
			_dbContext.Tasks.Add(task);
			await _dbContext.SaveChangesAsync();
		}
	}
}
