using Microsoft.EntityFrameworkCore;

namespace TMPTaskService.Infrastructure
{
	public class TMPDbContext(DbContextOptions<TMPDbContext> options) : DbContext(options)
	{
		public DbSet<Data.Models.Task> Tasks { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("Task");

			base.OnModelCreating(modelBuilder);
		}
	}
}
