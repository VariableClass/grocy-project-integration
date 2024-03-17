using Microsoft.Extensions.Options;
using VariableClass.Integration.GrocyProjects.Configs;

namespace VariableClass.Integration.GrocyProjects.Services.Authentik;

public class Authentik(IOptions<AuthentikConfig> configuration, HttpClient httpClient) : IAuthenticationService
{
    private const string OAuthKeyGrantType = "grant_type";
    private const string OAuthKeyClientId = "client_id";
    private const string OAuthKeyUsername = "username";
    private const string OAuthKeyPassword = "password";
    private const string OAuthKeyAccessToken = "access_token";
    private const string OAuthGrantTypeClientCredentials = "client_credentials";

    private readonly AuthentikConfig _configuration = configuration.Value;
    private readonly HttpClient _httpClient = httpClient;
    private string? _accessToken;

    public async Task<string> GetAccessTokenAsync()
    {
        // TODO: Add JWT validation here
        if (_accessToken == null)
        {
            var response = await _httpClient.PostAsync(
                $"{_configuration.InstanceUrl}{_configuration.TokenEndpoint}",
                new FormUrlEncodedContent(
                [
                    new KeyValuePair<string, string>(OAuthKeyGrantType, OAuthGrantTypeClientCredentials),
                    new KeyValuePair<string, string>(OAuthKeyClientId, _configuration.ClientId),
                    new KeyValuePair<string, string>(OAuthKeyUsername, _configuration.Username),
                    new KeyValuePair<string, string>(OAuthKeyPassword, _configuration.Password),
                ]));
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>()
            ?? throw new NullReferenceException();
            
            var accessToken = tokenResponse[OAuthKeyAccessToken]
                ?? throw new NullReferenceException();
            
            _accessToken = $"{accessToken}";
        }
        
        return _accessToken;
    }
}
