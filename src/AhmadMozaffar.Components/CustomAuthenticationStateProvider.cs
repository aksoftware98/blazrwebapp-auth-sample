using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{

	private HttpClient _httpClient;

	public CustomAuthenticationStateProvider(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		var response = await _httpClient.GetAsync("/api/identity");
		if (response.IsSuccessStatusCode)
		{
			var claims = await response.Content.ReadFromJsonAsync<IEnumerable<Claim>>();
			var identity = new ClaimsIdentity(claims, "Oidc");
			return new AuthenticationState(new ClaimsPrincipal(identity));
		}
		else
		{
			return new AuthenticationState(new ClaimsPrincipal()); // Empty no identity
		}
	}
}