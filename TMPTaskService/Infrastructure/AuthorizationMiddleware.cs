using TMPTaskService.Data.Interfaces;

namespace TMPTaskService.Infrastructure
{
	public class AuthorizationMiddleware(RequestDelegate next, IHostEnvironment env)
	{
		private readonly RequestDelegate _nextDelegate = next;
		private readonly IHostEnvironment _env = env;

		public async Task InvokeAsync(HttpContext context, IAuthenticationManager authManager)
		{
			if (_env.IsEnvironment("IntegrationTest"))
			{
				await _nextDelegate(context);
				return;
			}

			var endpoint = context.GetEndpoint();
			if (endpoint == null)
			{
				await _nextDelegate(context);
				return;
			}

			var requiresAuth = endpoint.Metadata.GetMetadata<RequireAuthorizationAttribute>() != null;
			if (!requiresAuth)
			{
				await _nextDelegate(context);
				return;
			}

			var authHeader  = context.Request.Headers.Authorization.FirstOrDefault();
			if (authHeader == null || !authHeader.StartsWith("Bearer "))
			{
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
			}

			var jwt = authHeader["Bearer ".Length..];
			if (jwt == null || !await authManager.IsJwtValidAsync(jwt))
			{
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
			}

			await _nextDelegate(context);
		}
	}
}
