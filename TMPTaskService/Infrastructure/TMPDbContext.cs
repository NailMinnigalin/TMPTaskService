using Microsoft.EntityFrameworkCore;
using TMPTaskService.Data.Models;

namespace TMPTaskService.Infrastructure
{
	public class TMPDbContext(DbContextOptions<TMPDbContext> options) : DbContext(options)
	{
		public DbSet<Data.Models.Task> Tasks { get; set; }
	}
}
