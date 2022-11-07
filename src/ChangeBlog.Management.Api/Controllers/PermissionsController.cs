using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Accounts.AccountExists;
using ChangeBlog.Application.UseCases.Accounts.GetAuthorizationState;
using ChangeBlog.Application.UseCases.ChangeLogs.ChangeLogLineExists;
using ChangeBlog.Application.UseCases.Products.ProductExists;
using ChangeBlog.Application.UseCases.Versions.VersionExists;
using ChangeBlog.Domain.Authorization;
using ChangeBlog.Management.Api.DTOs.Permissions;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Controllers;

[ApiController]
[Route("api/permissions")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[SwaggerControllerOrder(1)]
public class PermissionsController : ControllerBase
{
    private readonly IAccountExists _accountExists;
    private readonly IChangeLogLineExists _changeLogLineExists;
    private readonly IGetAuthorizationState _getAuthorizationState;
    private readonly IProductExists _productExists;
    private readonly IVersionExists _versionExists;

    public PermissionsController(IGetAuthorizationState getAuthorizationState, IAccountExists accountExists,
        IProductExists productExists, IVersionExists versionExists, IChangeLogLineExists changeLogLineExists)
    {
        _getAuthorizationState = getAuthorizationState;
        _accountExists = accountExists;
        _productExists = productExists;
        _versionExists = versionExists;
        _changeLogLineExists = changeLogLineExists;
    }

    [HttpGet(Name = "GetPermissions")]
    [SkipAuthorization]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourcePermissionsDto>> GetPermissions(
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

        return permissions.HasValue
            ? Ok(permissions.Value)
            : NotFound();
    }

    private async Task<Maybe<ResourcePermissionsDto>> GetAccountPermissions(Guid userId, Guid accountId)
    {
        var accountExists = await _accountExists.ExecuteAsync(accountId);
        if (!accountExists)
            return Maybe<ResourcePermissionsDto>.None;

        return new ResourcePermissionsDto
        {
            ResourceType = ResourceType.Account,
            ResourceId = accountId,
            CanRead = await IsPermittedAsync(Permission.ViewAccount),
            CanUpdate = await IsPermittedAsync(Permission.UpdateAccount),
            CanDelete = await IsPermittedAsync(Permission.DeleteAccount),
            SpecificPermissions = new Dictionary<string, bool>
            {
                ["canViewUsers"] = await IsPermittedAsync(Permission.ViewAccountUsers),
                ["canViewProducts"] = await IsPermittedAsync(Permission.ViewProduct),
                ["canCreateProduct"] = await IsPermittedAsync(Permission.AddOrUpdateProduct)
            }
        };

        async Task<bool> IsPermittedAsync(Permission permission)
        {
            var authState = await _getAuthorizationState
                .GetAuthStateByAccountIdAsync(userId, accountId, permission);

            return authState == AuthorizationState.Authorized;
        }
    }

    private async Task<Maybe<ResourcePermissionsDto>> GetProductPermissions(Guid userId, Guid productId)
    {
        var productExists = await _productExists.ExecuteAsync(productId);
        if (!productExists)
            return Maybe<ResourcePermissionsDto>.None;

        return new ResourcePermissionsDto
        {
            ResourceType = ResourceType.Product,
            ResourceId = productId,
            CanRead = await IsPermittedAsync(Permission.ViewProduct),
            CanUpdate = await IsPermittedAsync(Permission.AddOrUpdateProduct),
            CanDelete = await IsPermittedAsync(Permission.FreezeProduct),
            SpecificPermissions = new Dictionary<string, bool>
            {
                ["canFreeze"] = await IsPermittedAsync(Permission.FreezeProduct),
                ["canSeePendingChangeLogs"] = await IsPermittedAsync(Permission.ViewPendingChangeLogLines)
            }
        };

        async Task<bool> IsPermittedAsync(Permission permission)
        {
            var authState = await _getAuthorizationState
                .GetAuthStateByProductIdAsync(userId, productId, permission);

            return authState == AuthorizationState.Authorized;
        }
    }

    private async Task<Maybe<ResourcePermissionsDto>> GetVersionPermissions(Guid userId, Guid versionId)
    {
        var versionExists = await _versionExists.ExecuteAsync(versionId);
        if (!versionExists)
            return Maybe<ResourcePermissionsDto>.None;

        return new ResourcePermissionsDto
        {
            ResourceType = ResourceType.Version,
            ResourceId = versionId,
            CanRead = await IsPermittedAsync(Permission.ViewVersions),
            CanUpdate = await IsPermittedAsync(Permission.AddOrUpdateVersion),
            CanDelete = await IsPermittedAsync(Permission.DeleteVersion),
            SpecificPermissions = new Dictionary<string, bool>
            {
                ["canRelease"] = await IsPermittedAsync(Permission.ReleaseVersion)
            }
        };

        async Task<bool> IsPermittedAsync(Permission permission)
        {
            var authState = await _getAuthorizationState
                .GetAuthStateByVersionIdAsync(userId, versionId, permission);

            return authState == AuthorizationState.Authorized;
        }
    }

    private async Task<Maybe<ResourcePermissionsDto>> GetChangeLogPermissions(Guid userId, Guid changeLogLineId)
    {
        var changeLogLineExists = await _changeLogLineExists.ExecuteAsync(changeLogLineId);
        if (!changeLogLineExists)
            return Maybe<ResourcePermissionsDto>.None;

        return new ResourcePermissionsDto
        {
            ResourceType = ResourceType.ChangeLogLine,
            ResourceId = changeLogLineId,
            CanRead = await IsPermittedAsync(Permission.ViewChangeLogLines),
            CanUpdate = await IsPermittedAsync(Permission.AddOrUpdateChangeLogLine),
            CanDelete = await IsPermittedAsync(Permission.DeleteChangeLogLine),
            SpecificPermissions = new Dictionary<string, bool>
            {
                ["canMove"] = await IsPermittedAsync(Permission.MoveChangeLogLines),
                ["canViewPending"] = await IsPermittedAsync(Permission.ViewPendingChangeLogLines)
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