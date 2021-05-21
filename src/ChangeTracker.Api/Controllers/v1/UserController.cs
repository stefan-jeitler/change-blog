using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs.V1.Account;
using ChangeTracker.Api.DTOs.V1.Product;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Queries.GetProducts;
using ChangeTracker.Application.UseCases.Queries.GetUsers;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/user")]
    public class UserController : ControllerBase
    {
        private readonly IGetUserProducts _getUserProducts;
        private readonly IGetUsers _getUser;

        public UserController(IGetUserProducts getUserProducts, IGetUsers getUser)
        {
            _getUserProducts = getUserProducts;
            _getUser = getUser;
        }

        [HttpGet("products")]
        [NeedsPermission(Permission.ViewUserProducts)]
        public async Task<ActionResult> GetUserProductsAsync(ushort limit = UserProductQueryRequestModel.MaxLimit, 
            Guid? lastProductId = null,
            bool includeClosedProducts = false)
        {
            var userId = HttpContext.GetUserId();
            var requestModel = new UserProductQueryRequestModel(userId,
                lastProductId,
                limit,
                includeClosedProducts);

            var products = await _getUserProducts.ExecuteAsync(requestModel);

            return Ok(products.Select(ProductDto.FromResponseModel));
        }

        [HttpGet("info")]
        [NeedsPermission(Permission.ViewOwnUser)]
        public async Task<ActionResult> GetUserInfoAsync()
        {
            var userId = HttpContext.GetUserId();
            var user = await _getUser.ExecuteAsync(userId);

            return Ok(UserDto.FromResponseModel(user));
        }
    }
}