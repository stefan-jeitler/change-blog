using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ChangeBlog.Management.Api.Tests.Infrastructure;

[UsedImplicitly]
public record RopcFlowResponse
{
    [JsonPropertyName("access_token")] public string AccessToken { get; init; }

    [JsonPropertyName("token_type")] public string TokenType { get; init; }

    [JsonPropertyName("expires_in")] public string ExpiresInSeconds { get; init; }

    [JsonPropertyName("refresh_token")] public string RefreshToken { get; init; }
}

public class RopcFlowClient
{
    private readonly HttpClient _httpClient;

    public RopcFlowClient()
        : this(new HttpClient())
    {
    }

    public RopcFlowClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<RopcFlowResponse> AcquireToken(RopcFlowConfiguration config)
    {
        var tokenRequestMessage = new HttpRequestMessage(HttpMethod.Post, config.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = config.ClientId,
                ["username"] = config.Username,
                ["password"] = config.Password,
                ["scope"] = config.Scope
            })
        };

        var response = await _httpClient.SendAsync(tokenRequestMessage);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<RopcFlowResponse>();
    }
}