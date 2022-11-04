using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChangeBlog.Api.DTOs.V1.Product;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ChangeBlog.Api.Tests;

public class AccountControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;

    public AccountControllerTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
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
                "/api/v1/accounts/ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc/products?includeFreezed=true");
        var content = await response.Content.ReadFromJsonAsync<List<ProductDto>>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().HaveCount(2);
    }
}