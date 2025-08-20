using TMPTaskService.Data.Interfaces;

namespace TMPTaskService.Data.Implementations
{
	public class WebAuthenticationManager(HttpClient httpClient) : IAuthenticationManager
	{
		private readonly HttpClient _httpClient = httpClient;

		public async Task<bool> IsJwtValidAsync(string jwt)
		{
			try
            {   
                var response = await _httpClient.PostAsJsonAsync("/api/Authentication/ValidateJwt", new {jwt});

                if (!response.IsSuccessStatusCode)
                    return false;

                return await response.Content.ReadFromJsonAsync<bool>();
            }
            catch
            {
                return false;
            }
		}
	}
}
