using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TMPTaskService.Infrastructure;

namespace TMPTaskServiceIntegrationTests
{
	public class CustomWebApplicationFactory : WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TMPDbContext>));
				if (descriptor != null) services.Remove(descriptor);

				services.AddDbContext<TMPDbContext>(options =>
				{
					options.UseInMemoryDatabase("TestDb");
				});
			});
		}
	}
}
