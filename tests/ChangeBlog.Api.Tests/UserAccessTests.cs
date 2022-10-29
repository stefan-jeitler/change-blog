using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ChangeBlog.Api.Tests;

public class UserAccessTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;

    public UserAccessTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task UserAccess_ProductManagerClosesProduct_Unauthorized()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc01usr01"});

        var response = await client.PostAsync("/api/v1/products/139a2e54-e9be-4168-98b4-2839d9b3db04/close",
            new StringContent(""));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UserAccess_GetProductWithinAccountButNoPermission_Unauthorized()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc01usr01"});

        // product: t_ua_account_01_proj_02
        var response = await client.GetAsync("/api/v1/products/0614f8d6-8895-4c74-bcbe-8a3c26076e1b");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UserAccess_GetProductWithinAccountWithProductPermission_Authorized()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc01usr01"});

        // product: t_ua_account_01_proj_01
        var response = await client.GetAsync("/api/v1/products/139a2e54-e9be-4168-98b4-2839d9b3db04");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UserAccess_GetProductWithExplicitlyGrantedPermission_Authorized()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc02usr03"});

        // product: t_ua_account_01_proj_01
        var response = await client.GetAsync("/api/v1/products/35c5df1a-079e-4b8c-87c5-09b30e52a82f");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}