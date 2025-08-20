namespace TMPTaskService.Data.Interfaces
{
	public interface IAuthenticationManager
	{
		public Task<bool> IsJwtValidAsync(string jwt);
	}
}
