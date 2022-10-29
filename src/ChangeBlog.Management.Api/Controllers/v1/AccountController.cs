using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.DTOs.V1.User;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Accounts.GetAccounts;
using ChangeBlog.Application.UseCases.Accounts.GetRoles;
using ChangeBlog.Application.UseCases.Users.GetUsers;
using ChangeBlog.Domain.Authorization;
using ChangeBlog.Management.Api.DTOs;
using ChangeBlog.Management.Api.DTOs.V1.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Controllers.v1;

[ApiController]
[Route("api/v1/accounts")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[SwaggerControllerOrder(1)]
public class AccountController : ControllerBase
{
    [HttpGet(Name = "GetAccounts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SkipAuthorization]
    public async Task<ActionResult<List<AccountDto>>> GetAccountsAsync(
        [FromServices] IGetAccounts getAccounts)
    {
        var userId = HttpContext.GetUserId();
        var accounts = await getAccounts.ExecuteAsync(userId);

        return Ok(accounts.Select(AccountDto.FromResponseModel));
    }

    [HttpGet("{accountId:Guid}", Name = "GetAccount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [NeedsPermission(Permission.ViewAccount)]
    public async Task<ActionResult<AccountDto>> GetAccountAsync([FromServices] IGetAccount getAccount,
        Guid accountId)
    {
        var userId = HttpContext.GetUserId();
        var account = await getAccount.ExecuteAsync(userId, accountId);

        return Ok(AccountDto.FromResponseModel(account));
    }

    [HttpGet("{accountId:Guid}/users", Name = "GetAccountUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [NeedsPermission(Permission.ViewAccountUsers)]
    public async Task<ActionResult<List<UserDto>>> GetAccountUsersAsync(
        [FromServices] IGetUsers getUsers,
        Guid accountId,
        Guid? lastUserId = null,
        ushort limit = UsersQueryRequestModel.MaxLimit)
    {
        var userId = HttpContext.GetUserId();
        var requestModel = new UsersQueryRequestModel(userId,
            accountId,
            lastUserId,
            limit
        );

        var users = await getUsers.ExecuteAsync(requestModel);

        return Ok(users.Select(UserDto.FromResponseModel));
    }

    [HttpGet("roles", Name = "GetRoles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [SkipAuthorization]
    public async Task<ActionResult<List<RoleDto>>> GetRolesAsync([FromServices] IGetRoles getRoles,
        string filter = null,
        bool includePermissions = false)
    {
        var roles = await getRoles.ExecuteAsync(filter);

        return Ok(roles.Select(x => RoleDto.FromResponseModel(x, includePermissions)));
    }
}