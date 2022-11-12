using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared.DTOs.V1.Product;
using ChangeBlog.Management.Api.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ChangeBlog.Management.Api.Tests;

public class ProductControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;

    public ProductControllerTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetProduct_HappyPath_Successful()
    {
        // arrange
        var client = _factory.CreateClient();
        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // act
        var response = await client.GetAsync("/api/v1/products/139a2e54-e9be-4168-98b4-2839d9b3db04");
        var content = await response.Content.ReadFromJsonAsync<ProductDto>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Id.Should().Be(Guid.Parse("139a2e54-e9be-4168-98b4-2839d9b3db04"));
    }

    [Fact]
    public async Task FreezeProduct_HappyPath_Returns200()
    {
        // arrange
        var client = _factory.CreateClient();
        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // act
        var response = await client.PostAsync("api/v1/products/139a2e54-e9be-4168-98b4-2839d9b3db04/freeze", null!);

        // arrange
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}