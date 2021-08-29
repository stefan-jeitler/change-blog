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
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.SwaggerUI;
using ChangeBlog.Application.UseCases;
using ChangeBlog.Application.UseCases.Queries.GetProducts;
using ChangeBlog.Application.UseCases.Queries.GetUsers;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/user")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status403Forbidden)]
    [SwaggerControllerOrder(2)]
    public class UserController : ControllerBase
    {
        private readonly IGetUsers _getUser;
        private readonly IGetUserProducts _getUserProducts;

        public UserController(IGetUserProducts getUserProducts, IGetUsers getUser)
        {
            _getUserProducts = getUserProducts;
            _getUser = getUser;
        }

        [HttpGet("products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [SkipAuthorization]
        public async Task<ActionResult<List<ProductDto>>> GetUserProductsAsync(Guid? lastProductId = null,
            [Range(1, UserProductQueryRequestModel.MaxLimit)]
            ushort limit = UserProductQueryRequestModel.MaxLimit,
            bool includeClosed = false)
        {
            var userId = HttpContext.GetUserId();
            var requestModel = new UserProductQueryRequestModel(userId,
                lastProductId,
                limit,
                includeClosed);

            var products = await _getUserProducts.ExecuteAsync(requestModel);

            return Ok(products.Select(ProductDto.FromResponseModel));
        }

        [HttpGet("info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SkipAuthorization]
        public async Task<ActionResult<UserDto>> GetUserInfoAsync()
        {
            var userId = HttpContext.GetUserId();
            var user = await _getUser.ExecuteAsync(userId);

            return Ok(UserDto.FromResponseModel(user));
        }
    }
}
