namespace TMPTaskService.Infrastructure
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RequireAuthorizationAttribute : Attribute
	{
	}
}
