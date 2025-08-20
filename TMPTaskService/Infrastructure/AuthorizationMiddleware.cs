using TMPTaskService.Data.Interfaces;

namespace TMPTaskService.Infrastructure
{
	public class AuthorizationMiddleware
	{
		private readonly RequestDelegate _nextDelegate;

		public AuthorizationMiddleware(RequestDelegate next)
		{
			_nextDelegate = next;
		}

		public async Task InvokeAsync(HttpContext context, IAuthenticationManager authManager)
		{
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
