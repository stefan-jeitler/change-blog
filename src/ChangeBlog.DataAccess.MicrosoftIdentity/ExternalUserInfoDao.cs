using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using CSharpFunctionalExtensions;
using Microsoft.Identity.Web;

namespace ChangeBlog.DataAccess.MicrosoftIdentity;

public class ExternalUserInfoDao : IExternalUserInfoDao
{
    private const string IdentityProvider = "MicrosoftIdentityPlatform";

    private readonly HttpClient _httpClient;
    private readonly string[] _scopes = {"openid", "profile", "email", "offline_access"};
    private readonly ITokenAcquisition _tokenAcquisition;

    public ExternalUserInfoDao(ITokenAcquisition tokenAcquisition, HttpClient httpClient)
    {
        _tokenAcquisition = tokenAcquisition;
        _httpClient = httpClient;
    }

    public async Task<UserInfo> GetUserInfoAsync()
    {
        var token = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
        var message = CreateMessage(token, "/oidc/userinfo");

        var response = await _httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();

        var userDto = await response.Content.ReadFromJsonAsync<UserInfoDto>();

        return CreateUserInfo(userDto);
    }

    public async Task<Maybe<UserPhoto>> GetUserPhotoAsync()
    {
        var token = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
        var message = CreateMessage(token, "/v1.0/me/photo/$value");

        var response = await _httpClient.SendAsync(message);

        if(response.StatusCode == HttpStatusCode.NotFound)
            return Maybe<UserPhoto>.None;

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/png";
        var photo = await response.Content.ReadAsByteArrayAsync();
        
        return Maybe<UserPhoto>.From(new UserPhoto(contentType, photo));
    }

    private static UserInfo CreateUserInfo(UserInfoDto userDto)
    {
        if (userDto is null) throw new ArgumentNullException(nameof(userDto));

        return new UserInfo(userDto.Subject,
            userDto.Name,
            userDto.GivenName,
            userDto.FamilyName,
            userDto.Email,
            IdentityProvider);
    }

    private static HttpRequestMessage CreateMessage(string token, string uri)
    {
        return new HttpRequestMessage(HttpMethod.Get, uri)
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", token)
            }
        };
    }
}