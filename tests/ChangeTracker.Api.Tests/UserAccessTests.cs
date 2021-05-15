using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ChangeTracker.Api.Tests
{
    public class UserAccessTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public UserAccessTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task UserAccess_PlatformManager_CanViewRoles()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] { "acc01usr02" });

            var response = await client.GetAsync("/api/roles?includePermissions=false");

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task UserAccess_DefaultUser_Unauthorized()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] { "acc01usr01" });

            var response = await client.GetAsync("/api/roles?includePermissions=false");

            response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }

        [Fact]
        public async Task UserAccess_UnknownApiKey_NotAuthenticated()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] { "some-random-api-key" });

            var response = await client.GetAsync("/api/roles");

            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task UserAccess_NoApiKeyPresent_NotAuthenticated()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/roles");

            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task UserAccess_GetProjectWithinAccountButNoPermission_Unauthorized()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] { "acc01usr01" });

            // project: t_ua_account_01_proj_02
            var response = await client.GetAsync("/api/v1/projects/0614f8d6-8895-4c74-bcbe-8a3c26076e1b");

            response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }

        [Fact]
        public async Task UserAccess_GetProjectWithinAccountWithProjectPermission_Authorized()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] { "acc01usr01" });

            // project: t_ua_account_01_proj_01
            var response = await client.GetAsync("/api/v1/projects/139a2e54-e9be-4168-98b4-2839d9b3db04");

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task UserAccess_GetProjectWithExplicitlyGrantedPermission_Authorized()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] { "acc02usr03" });

            // project: t_ua_account_01_proj_01
            var response = await client.GetAsync("/api/v1/projects/35c5df1a-079e-4b8c-87c5-09b30e52a82f");

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
    }
}
