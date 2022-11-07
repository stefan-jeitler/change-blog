using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Management.Api.DTOs.Permissions;
using ChangeBlog.Management.Api.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ChangeBlog.Management.Api.Tests;

public class PermissionsControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;

    public PermissionsControllerTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Permissions_UserIsPlatformManager_IsFullyPermitted()
    {
        // arrange
        var client = _factory.CreateClient();
        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // act
        var response =
            await client.GetAsync(
                "/api/permissions?ResourceType=Account&ResourceId=ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
        var content = await response.Content.Deserialize<ResourcePermissionsDto>();

        // assert
        content.Should().NotBeNull();
        content.ResourceId.Should().Be("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
        content.ResourceType.Should().Be(ResourceType.Account);
        content!.CanDelete.Should().BeTrue();
        content.CanRead.Should().BeTrue();
        content.CanUpdate.Should().BeTrue();
        content.SpecificPermissions.Should().ContainKey("canViewUsers").WhoseValue.Should().BeTrue();
        content.SpecificPermissions.Should().ContainKey("canCreateProduct").WhoseValue.Should().BeTrue();
    }

    [Fact]
    public async Task Permissions_NotExistingAccountId_ReturnsNoPermission()
    {
        // arrange
        var client = _factory.CreateClient();
        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var notExistingAccountId = Guid.Parse("06D06450-5974-4C4C-8751-AD611314E5B0");

        // act
        var response =
            await client.GetAsync(
                $"/api/permissions?ResourceType=Account&ResourceId={notExistingAccountId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Permissions_InvalidResourceType_BadRequest()
    {
        // arrange
        var client = _factory.CreateClient();
        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // act
        var response =
            await client.GetAsync(
                "/api/permissions?ResourceType=FooBar&ResourceId=ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
        var content = await response.Content.Deserialize<ErrorResponse>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        var resourceTypeError = content.Errors.First();
        resourceTypeError.Messages.Should().OnlyContain(x => x == "The value 'FooBar' is not valid for ResourceType");
        resourceTypeError.Property.Should().Be("resourceType");
    }

    [Fact]
    public async Task Permissions_InvalidResourceId_BadRequest()
    {
        // arrange
        var client = _factory.CreateClient();
        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // act
        var response =
            await client.GetAsync(
                "/api/permissions?ResourceType=Account&ResourceId=00000000-0000-0000-0000-000000000000");
        var content = await response.Content.Deserialize<ErrorResponse>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        var resourceTypeError = content.Errors.First();
        resourceTypeError.Messages.Should().OnlyContain(x => x == "'Resource Id' must not be empty");
        resourceTypeError.Property.Should().Be("resourceId");
    }
}