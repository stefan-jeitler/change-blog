using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using Microsoft.Identity.Web;

namespace ChangeBlog.DataAccess.MicrosoftIdentity;

public class ExternalUserInfoDao : IExternalUserInfoDao
{
    private const string IdentityProvider = "MicrosoftIdentityPlatform";

    private readonly HttpClient _httpClient;
    private readonly string[] _scopes = { "openid", "profile", "email", "offline_access" };
    private readonly ITokenAcquisition _tokenAcquisition;

    public ExternalUserInfoDao(ITokenAcquisition tokenAcquisition, HttpClient httpClient)
    {
        _tokenAcquisition = tokenAcquisition;
        _httpClient = httpClient;
    }

    public async Task<UserInfo> GetAsync()
    {
        var token = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
        var message = CreateMessage(token);

        var response = await _httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();

        var userDto = await response.Content.ReadFromJsonAsync<UserInfoDto>();

        return CreateUserInfo(userDto);
    }

    private static UserInfo CreateUserInfo(UserInfoDto userDto)
    {
        if (userDto is null)
        {
            throw new ArgumentNullException(nameof(userDto));
        }

        return new UserInfo(userDto.Subject,
            userDto.Name,
            userDto.GivenName,
            userDto.FamilyName,
            userDto.Email,
            IdentityProvider);
    }

    private static HttpRequestMessage CreateMessage(string token)
    {
        return new HttpRequestMessage(HttpMethod.Get, string.Empty)
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", token)
            }
        };
    }
}