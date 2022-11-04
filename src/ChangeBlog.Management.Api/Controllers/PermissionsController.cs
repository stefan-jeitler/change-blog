using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Accounts.GetAuthorizationState;
using ChangeBlog.Domain.Authorization;
using ChangeBlog.Management.Api.DTOs.Permissions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Controllers;

[ApiController]
[Route("api/permission")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[SwaggerControllerOrder(1)]
public class PermissionsController : ControllerBase
{
    private readonly IGetAuthorizationState _getAuthorizationState;

    public PermissionsController(IGetAuthorizationState getAuthorizationState)
    {
        _getAuthorizationState = getAuthorizationState;
    }

    [HttpGet("permissions", Name = "GetPermissions")]
    [SkipAuthorization]
    [ProducesResponseType(typeof(ResourcePermissionsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPermissions(
        [FromQuery] RequestPermissionsDto requestDto)
    {
        var userId = HttpContext.GetUserId();

        var permissions = requestDto.ResourceType switch
        {
            ResourceType.Account => await GetAccountPermissions(userId, requestDto.ResourceId),
            ResourceType.Product => await GetProductPermissions(userId, requestDto.ResourceId),
            ResourceType.Version => await GetVersionPermissions(userId, requestDto.ResourceId),
            ResourceType.ChangeLogLine => await GetChangeLogPermissions(userId, requestDto.ResourceId),
            _ => throw new ArgumentOutOfRangeException(nameof(ResourceType))
        };

        return Ok(permissions);
    }

    private async Task<ResourcePermissionsDto> GetAccountPermissions(Guid accountId, Guid userId)
    {
        return new ResourcePermissionsDto
        {
            ResourceType = ResourceType.Account,
            ResourceId = accountId,
            CanCreate = true,
            CanRead = await IsPermittedAsync(Permission.ViewAccount),
            CanUpdate = await IsPermittedAsync(Permission.UpdateAccount),
            CanDelete = await IsPermittedAsync(Permission.DeleteAccount),
            SpecificPermissions = new Dictionary<string, bool>
            {
                ["CanViewUsers"] = await IsPermittedAsync(Permission.ViewAccountUsers)
            }
        };

        async Task<bool> IsPermittedAsync(Permission permission)
        {
            var authState = await _getAuthorizationState
                .GetAuthStateByAccountIdAsync(userId, accountId, permission);

            return authState == AuthorizationState.Authorized;
        }
    }

    private async Task<ResourcePermissionsDto> GetProductPermissions(Guid userId, Guid productId)
    {
        var addOrUpdate = await IsPermittedAsync(Permission.AddOrUpdateProduct);

        return new ResourcePermissionsDto
        {
            ResourceType = ResourceType.Product,
            ResourceId = productId,
            CanCreate = addOrUpdate,
            CanRead = await IsPermittedAsync(Permission.ViewProduct),
            CanUpdate = addOrUpdate,
            CanDelete = await IsPermittedAsync(Permission.ViewProduct),
            SpecificPermissions = new Dictionary<string, bool>
            {
                ["CanFreeze"] = await IsPermittedAsync(Permission.FreezeProduct)
            }
        };

        async Task<bool> IsPermittedAsync(Permission permission)
        {
            var authState = await _getAuthorizationState
                .GetAuthStateByProductIdAsync(userId, productId, permission);

            return authState == AuthorizationState.Authorized;
        }
    }

    private async Task<ResourcePermissionsDto> GetVersionPermissions(Guid userId, Guid versionId)
    {
        var addOrUpdate = await IsPermittedAsync(Permission.AddOrUpdateVersion);
        return new ResourcePermissionsDto
        {
            ResourceType = ResourceType.Version,
            ResourceId = versionId,
            CanCreate = addOrUpdate,
            CanRead = await IsPermittedAsync(Permission.ViewVersions),
            CanUpdate = addOrUpdate,
            CanDelete = await IsPermittedAsync(Permission.DeleteVersion),
            SpecificPermissions = new Dictionary<string, bool>
            {
                ["CanRelease"] = await IsPermittedAsync(Permission.ReleaseVersion)
            }
        };

        async Task<bool> IsPermittedAsync(Permission permission)
        {
            var authState = await _getAuthorizationState
                .GetAuthStateByVersionIdAsync(userId, versionId, permission);

            return authState == AuthorizationState.Authorized;
        }
    }

    private async Task<ResourcePermissionsDto> GetChangeLogPermissions(Guid userId, Guid changeLogLineId)
    {
        var addOrUpdate = await IsPermittedAsync(Permission.AddOrUpdateChangeLogLine);
        return new ResourcePermissionsDto
        {
            ResourceType = ResourceType.ChangeLogLine,
            ResourceId = changeLogLineId,
            CanCreate = addOrUpdate,
            CanRead = await IsPermittedAsync(Permission.ViewChangeLogLines),
            CanUpdate = addOrUpdate,
            CanDelete = await IsPermittedAsync(Permission.DeleteChangeLogLine),
            SpecificPermissions = new Dictionary<string, bool>
            {
                ["CanMove"] = await IsPermittedAsync(Permission.MoveChangeLogLines),
                ["CanViewPending"] = await IsPermittedAsync(Permission.ViewPendingChangeLogLines)
            }
        };

        async Task<bool> IsPermittedAsync(Permission permission)
        {
            var authState = await _getAuthorizationState
                .GetAuthStateByChangeLogLineIdAsync(userId, changeLogLineId, permission);

            return authState == AuthorizationState.Authorized;
        }
    }
}