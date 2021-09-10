using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess.ExternalIdentity;
using Microsoft.Identity.Web;

namespace ChangeBlog.DataAccess.MicrosoftIdentity
{
    public class ExternalUserInfoDao : IExternalUserInfoDao
    {
        private const string IdentityProvider = "MicrosoftIdentityPlatform";
        private readonly string _baseUrl;

        private readonly HttpClient _httpClient;
        private readonly string[] _scopes = {"openid", "profile", "email", "offline_access"};
        private readonly ITokenAcquisition _tokenAcquisition;

        public ExternalUserInfoDao(ITokenAcquisition tokenAcquisition, HttpClient httpClient, string baseUrl)
        {
            _tokenAcquisition = tokenAcquisition;
            _httpClient = httpClient;
            _baseUrl = baseUrl;
        }

        public async Task<UserInfo> GetAsync()
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);

            var userInfoEndpoint = new Uri($"{_baseUrl.TrimEnd('/')}/userinfo");
            var message = CreateMessage(userInfoEndpoint, token);

            var response = await _httpClient.SendAsync(message);
            var userDto = await response.Content.ReadFromJsonAsync<UserInfoDto>();

            if (userDto is null)
                throw new ArgumentNullException(nameof(userDto));

            return new UserInfo(userDto.Subject,
                userDto.Name,
                userDto.GivenName,
                userDto.FamilyName,
                userDto.Email,
                IdentityProvider);
        }

        private static HttpRequestMessage CreateMessage(Uri userInfoEndpoint, string token) =>
            new(HttpMethod.Get, userInfoEndpoint)
            {
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", token)
                }
            };
    }
}