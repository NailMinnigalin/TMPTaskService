using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TMPTaskServiceIntegrationTests
{
	public class CustomWebApplicationFactory : WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("IntegrationTest");
		}
	}
}
