using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.DTOs.V1.User;
using ChangeBlog.Management.Api.DTOs;
using ChangeBlog.Management.Api.DTOs.V1.Account;
using ChangeBlog.Management.Api.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ChangeBlog.Management.Api.Tests;

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
        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // act
        var response = await client.GetAsync("/api/v1/accounts/");
        var content = await response.Content.ReadFromJsonAsync<List<AccountDto>>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().HaveCount(2);
        content.Should().Contain(x => x.Id == Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc"));
        content.Should().Contain(x => x.Id == Guid.Parse("851c1aee-b051-462c-86b3-0e8623d2ce23"));
    }

    [Fact]
    public async Task Account_GetAccount_Successful()
    {
        // arrange
        var client = _factory.CreateClient();
        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // act
        var response = await client.GetAsync("/api/v1/accounts/ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
        var content = await response.Content.ReadFromJsonAsync<AccountDto>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Id.Should().Be(Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc"));
    }

    [Fact]
    public async Task Account_GetAccountUsers_Successful()
    {
        // arrange
        var client = _factory.CreateClient();
        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // act
        var response = await client.GetAsync("/api/v1/accounts/ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc/users");
        var content = await response.Content.ReadFromJsonAsync<List<UserDto>>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().HaveCount(3);
    }

    [Fact]
    public async Task Account_GetRoles_Successful()
    {
        // arrange
        var client = _factory.CreateClient();
        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // act
        var response = await client.GetAsync("/api/v1/accounts/roles");
        var content = await response.Content.ReadFromJsonAsync<List<RoleDto>>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().HaveCount(7);
    }

    [Fact]
    public async Task UserAccess_OutdatedToken_NotAuthenticated()
    {
        var client = _factory.CreateClient();
        const string outdatedJwt =
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJpc3MiOiJodHRwczovL2NoYW5nZWJsb2dpZGVudGl0eS5iMmNsb2dpbi5jb20vNjllZDUyNmUtMzMyYy00OGY4LThhNWItNjczYTNiOTlhZGVjL3YyLjAvIiwiZXhwIjoxNjY3MDYzNzk5LCJuYmYiOjE2NjcwNjAxOTksImF1ZCI6ImIxNTU2NzgyLTBmY2QtNGYxNS1iOWZiLTA1MDMxNzliNzdlYyIsImlkcCI6IkxvY2FsQWNjb3VudCIsInN1YiI6IjFmMjZmZjg2LTYwZjEtNDc1My04NjgwLTBmZjRiNTI5ODhmYSIsImdpdmVuX25hbWUiOiJTdGVmYW4iLCJmYW1pbHlfbmFtZSI6IkplaXRsZXIiLCJlbWFpbHMiOlsiY2hhbmdlYmxvZ0BvdXRsb29rLmNvbSJdLCJ0ZnAiOiJCMkNfMV9ST1BDIiwic2NwIjoiQXBpLkFjY2Vzcy5BbGwiLCJhenAiOiI1NDg4MjhkOC1kNmVjLTQzZTQtYTljOS1hMTI2NjQwM2YwZjIiLCJ2ZXIiOiIxLjAiLCJpYXQiOjE2NjcwNjAxOTl9.HrbxHcKFabyG6u1C7AtDffokrpjFE057g-bP8GvEBAVQ3hCcSNWsO8SSou-1lAsGQYtuYxpQ4YytaVfeACoL3s6-DBPMJp_hJfBrEpsZBnS7M15ODdlo9m2oPD9Gtvf47E2awogvRRapM4iMPj6GgpoZAbf24qDDRiowcUgoLZNGvQsImTxQYX3v6UmN58YkEX1C3PSzuxzgzprqkMwnXRcILYMosHAABQydkyrE347a8B2C6n52GhjgSz3oOUAqivMgDcSwPwZRMJm92cc_DZhqEGacojN5kgGLVdwq_5CC3TExsmx7ahouNcii5BHWqvL8E_72EGAl15FYbwXQ1w";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", outdatedJwt);

        var response = await client.GetAsync("/api/v1/accounts/roles");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UserAccess_NoApiKeyPresent_NotAuthenticated()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/accounts/roles");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UserAccess_PlatformManager_CanViewRoles()
    {
        var client = _factory.CreateClient();
        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("/api/v1/accounts/roles?includePermissions=false");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateAccount_AccountAlreadyExists_UnprocessableEntityResult()
    {
        var client = _factory.CreateClient();
        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de"));

        var jsonBody = JsonSerializer.Serialize(
            new CreateAccountDto {Name = "ChangeBlog.Management.Api.Tests"});
        var response =
            await client.PostAsync("api/v1/accounts/", new StringContent(jsonBody, Encoding.UTF8,
                "application/json"));
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        error.Should().NotBeNull();
        error!.Errors.Should().HaveCount(1);
        var firstError = error.Errors.First();
        firstError.Messages.First().Should().Be("Der Name ist bereits vergeben");
        firstError.Property.Should().Be("name");
    }
}