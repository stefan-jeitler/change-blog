using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Api.DTOs.V1.Account;
using ChangeBlog.Api.DTOs.V1.Product;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ChangeBlog.Api.Tests
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
            // arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc01usr02"});

            // act
            var response = await client.GetAsync("/api/v1/accounts/");
            var content = await response.Content.ReadFromJsonAsync<List<AccountDto>>();

            // assert
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            content.Should().HaveCount(1);
            content.Should().Contain(x => x.Id == Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc"));
        }

        [Fact]
        public async Task Account_GetAccount_Successful()
        {
            // arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc01usr02"});

            // act
            var response = await client.GetAsync("/api/v1/accounts/ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
            var content = await response.Content.ReadFromJsonAsync<AccountDto>();

            // assert
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            content.Id.Should().Be(Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc"));
        }

        [Fact]
        public async Task Account_GetAccountProducts_ReturnsTwoProducts()
        {
            // arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc01usr02"});

            // act
            var response =
                await client.GetAsync(
                    "/api/v1/accounts/ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc/products?includeClosed=true");
            var content = await response.Content.ReadFromJsonAsync<List<ProductDto>>();

            // assert
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            content.Should().HaveCount(2);
        }

        [Fact]
        public async Task Account_GetAccountUsers_Successful()
        {
            // arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc01usr02"});

            // act
            var response = await client.GetAsync("/api/v1/accounts/ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc/users");
            var content = await response.Content.ReadFromJsonAsync<List<UserDto>>();

            // assert
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            content.Should().HaveCount(2);
        }

        [Fact]
        public async Task Account_GetRoles_Successful()
        {
            // arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc01usr02"});

            // act
            var response = await client.GetAsync("/api/v1/accounts/roles");
            var content = await response.Content.ReadFromJsonAsync<List<RoleDto>>();

            // assert
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            content.Should().HaveCount(7);
        }
    }
}
