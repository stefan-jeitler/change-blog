using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Api.DTOs.v1.Account;
using ChangeTracker.Api.DTOs.v1.Project;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NodaTime;
using Xunit;

namespace ChangeTracker.Api.Tests
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public AccountControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Account_GetAccounts_Successful()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] { "acc01usr02" });
            
            var response = await client.GetAsync("/api/v1/accounts/");
            var content = await response.Content.ReadFromJsonAsync<List<AccountDto>>();

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            content.Should().HaveCount(1);
            content.Should().Contain(x => x.Id == Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc"));
        }

        [Fact]
        public async Task Account_GetAccount_Successful()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] { "acc01usr02" });
            
            var response = await client.GetAsync("/api/v1/accounts/ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
            var content = await response.Content.ReadFromJsonAsync<AccountDto>();

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            content.Id.Should().Be(Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc"));
        }

        [Fact]
        public async Task Account_GetAccountProjects_Successful()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] { "acc01usr02" });
            
            var response = await client.GetAsync("/api/v1/accounts/ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc/projects");
            var content = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            content.Should().HaveCount(2);
        }

        [Fact]
        public async Task Account_GetAccountUsers_Successful()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] { "acc01usr02" });
            
            var response = await client.GetAsync("/api/v1/accounts/ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc/users");
            var content = await response.Content.ReadFromJsonAsync<List<UserDto>>();

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            content.Should().HaveCount(3);
        }
    }
}
