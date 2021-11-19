using System.Net;
using System.Threading.Tasks;
using ChangeBlog.Api.Authentication;
using ChangeBlog.Api.Shared.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace ChangeBlog.Api.Tests;

public class HomeControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;

    public HomeControllerTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task IndexEndpoint_DeserializeApiInfo_SuccessfullyDeserialized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/info");

        var apiInfo = JsonConvert.DeserializeObject<ApiInfo>(await response.Content.ReadAsStringAsync());
        apiInfo.Should().NotBeNull();
    }

    [Fact]
    public async Task IndexEndpoint_Exists()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangeLogEndpoint_Exists()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("api/changes");
            
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}