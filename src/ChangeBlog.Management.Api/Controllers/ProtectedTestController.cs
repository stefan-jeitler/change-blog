using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace ChangeBlog.Management.Api.Controllers
{
    [ApiController]
    [Route("api/protected")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ProtectedTestController : ControllerBase
    {

        private readonly ITokenAcquisition _tokenAcquisition;

        public ProtectedTestController(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }


        [HttpGet]
        public string GetProtectedString() => "Protected-String";


        [HttpGet("userprofile")]
        public async Task<ActionResult> GetUserProfileAsync()
        {
            try
            {
                var token = await _tokenAcquisition.GetAccessTokenForUserAsync(new[]
                    {"openid", "profile", "email", "offline_access"});

                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri("https://graph.microsoft.com/oidc/"),
                    DefaultRequestHeaders =
                    {
                        Authorization = new AuthenticationHeaderValue("Bearer", token)
                    }
                };

                var response = await httpClient.GetAsync("userinfo");
                var content = await response.Content.ReadAsStringAsync();

                return Ok(content);
            }
            catch (Exception e)
            {
            }

            return Conflict();
        }
    }
}