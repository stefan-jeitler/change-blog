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
        private readonly string[] _scopes = {"openid", "profile", "email", "offline_access"};
        private readonly HttpClient _httpClient;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly string _baseUrl;

        public ExternalUserInfoDao(ITokenAcquisition tokenAcquisition, HttpClient httpClient, string baseUrl)
        {
            _tokenAcquisition = tokenAcquisition;
            _httpClient = httpClient;
            _baseUrl = baseUrl;
        }

        public async Task<UserInfo> GetAsync()
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
            
            var userInfoUrl = new Uri($"{_baseUrl.TrimEnd('/')}/userinfo");
            var message = new HttpRequestMessage(HttpMethod.Get, userInfoUrl)
            {
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", token)
                }
            };

            var response = await _httpClient.SendAsync(message);
            var userDto = await response.Content.ReadFromJsonAsync<UserInfoDto>();

            if (userDto is null)
                throw new ArgumentNullException(nameof(userDto));

            return new UserInfo(userDto.Subject,
                userDto.Name,
                userDto.GivenName,
                userDto.FamilyName,
                userDto.Email);
        }
    }
}