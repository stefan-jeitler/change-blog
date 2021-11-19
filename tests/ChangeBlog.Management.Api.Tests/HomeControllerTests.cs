using System.Threading.Tasks;
using ChangeBlog.Api.Shared.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace ChangeBlog.Management.Api.Tests;

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
        apiInfo.Environment.Should().Be("Development");
    }
}