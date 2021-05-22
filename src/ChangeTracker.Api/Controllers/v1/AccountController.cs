﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.V1.Account;
using ChangeTracker.Api.DTOs.V1.Product;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Queries.GetAccounts;
using ChangeTracker.Application.UseCases.Queries.GetProducts;
using ChangeTracker.Application.UseCases.Queries.GetRoles;
using ChangeTracker.Application.UseCases.Queries.GetUsers;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IGetAccounts _getAccounts;
        private readonly IGetAccountProducts _getAccountProducts;
        private readonly IGetUsers _getUsers;

        public AccountController(IGetAccountProducts getAccountProducts, IGetAccounts getAccounts, IGetUsers getUsers)
        {
            _getAccountProducts = getAccountProducts;
            _getAccounts = getAccounts;
            _getUsers = getUsers;
        }

        [HttpGet]
        [NeedsPermission(Permission.ViewAccount)]
        public async Task<ActionResult<List<AccountDto>>> GetAccountsAsync()
        {
            var userId = HttpContext.GetUserId();
            var accounts = await _getAccounts.ExecuteAsync(userId);

            return Ok(accounts.Select(AccountDto.FromResponseModel));
        }

        [HttpGet("{accountId:Guid}")]
        [NeedsPermission(Permission.ViewAccount)]
        public async Task<ActionResult<AccountDto>> GetAccountAsync(Guid accountId)
        {
            var userId = HttpContext.GetUserId();
            var account = await _getAccounts.ExecuteAsync(userId, accountId);

            return Ok(AccountDto.FromResponseModel(account));
        }

        [HttpGet("{accountId:Guid}/users")]
        [NeedsPermission(Permission.ViewAccountUsers)]
        public async Task<ActionResult<List<UserDto>>> GetUsersAsync(Guid accountId,
            Guid? lastUserId = null,
            ushort limit = UsersQueryRequestModel.MaxLimit)
        {
            var requestModel = new UsersQueryRequestModel(HttpContext.GetUserId(),
                accountId,
                lastUserId,
                limit
            );

            var products = await _getUsers.ExecuteAsync(requestModel);

            return Ok(products.Select(UserDto.FromResponseModel));
        }

        [HttpGet("{accountId:Guid}/products")]
        [NeedsPermission(Permission.ViewAccountProducts)]
        public async Task<ActionResult<List<ProductDto>>> GetProductsAsync(Guid accountId,
            Guid? lastProductId = null,
            ushort limit = AccountProductQueryRequestModel.MaxLimit,
            bool includeClosedProducts = false)
        {
            var requestModel = new AccountProductQueryRequestModel(HttpContext.GetUserId(),
                accountId,
                lastProductId,
                limit,
                includeClosedProducts
            );

            var products = await _getAccountProducts.ExecuteAsync(requestModel);

            return Ok(products.Select(ProductDto.FromResponseModel));
        }

        [HttpGet("roles")]
        [NeedsPermission(Permission.ViewRoles)]
        public async Task<ActionResult<List<RoleDto>>> GetRolesAsync([FromServices] IGetRoles getRoles,
            string filter = null,
            bool includePermissions = false)
        {
            var roles = await getRoles.ExecuteAsync(filter);

            return Ok(roles.Select(x => RoleDto.FromResponseModel(x, includePermissions)));
        }
    }
}