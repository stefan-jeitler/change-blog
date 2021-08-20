using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Authorization;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Api.DTOs.V1.Account;
using ChangeBlog.Api.DTOs.V1.Product;
using ChangeBlog.Api.Extensions;
using ChangeBlog.Api.SwaggerUI;
using ChangeBlog.Application.UseCases.Queries.GetAccounts;
using ChangeBlog.Application.UseCases.Queries.GetProducts;
using ChangeBlog.Application.UseCases.Queries.GetRoles;
using ChangeBlog.Application.UseCases.Queries.GetUsers;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/accounts")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status403Forbidden)]
    [SwaggerControllerOrder(1)]
    public class AccountController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SkipAuthorization]
        public async Task<ActionResult<List<AccountDto>>> GetAccountsAsync(
            [FromServices] IGetAccounts getAccounts)
        {
            var userId = HttpContext.GetUserId();
            var accounts = await getAccounts.ExecuteAsync(userId);

            return Ok(accounts.Select(AccountDto.FromResponseModel));
        }

        [HttpGet("{accountId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [NeedsPermission(Permission.ViewAccount)]
        public async Task<ActionResult<AccountDto>> GetAccountAsync([FromServices] IGetAccount getAccount,
            Guid accountId)
        {
            var userId = HttpContext.GetUserId();
            var account = await getAccount.ExecuteAsync(userId, accountId);

            return Ok(AccountDto.FromResponseModel(account));
        }

        [HttpGet("{accountId:Guid}/users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
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

            var products = await getUsers.ExecuteAsync(requestModel);

            return Ok(products.Select(UserDto.FromResponseModel));
        }

        [HttpGet("{accountId:Guid}/products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [NeedsPermission(Permission.ViewAccountProducts)]
        public async Task<ActionResult<List<ProductDto>>> GetAccountProductsAsync(
            [FromServices] IGetAccountProducts getAccountProducts,
            Guid accountId,
            Guid? lastProductId = null,
            [Range(1, AccountProductQueryRequestModel.MaxLimit)]
            ushort limit = AccountProductQueryRequestModel.MaxLimit,
            bool includeClosed = false)
        {
            var userId = HttpContext.GetUserId();
            var requestModel = new AccountProductQueryRequestModel(userId,
                accountId,
                lastProductId,
                limit,
                includeClosed
            );

            var products = await getAccountProducts.ExecuteAsync(requestModel);

            return Ok(products.Select(ProductDto.FromResponseModel));
        }

        [HttpGet("roles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [SkipAuthorization]
        public async Task<ActionResult<List<RoleDto>>> GetRolesAsync([FromServices] IGetRoles getRoles,
            string filter = null,
            bool includePermissions = false)
        {
            var roles = await getRoles.ExecuteAsync(filter);

            return Ok(roles.Select(x => RoleDto.FromResponseModel(x, includePermissions)));
        }
    }
}