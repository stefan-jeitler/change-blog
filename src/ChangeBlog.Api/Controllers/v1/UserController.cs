using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.DTOs.V1.Product;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Products.GetProducts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Controllers.v1;

[ApiController]
[Route("api/v1/user")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[SwaggerControllerOrder(2)]
public class UserController : ControllerBase
{
    private readonly IGetUserProducts _getUserProducts;

    public UserController(IGetUserProducts getUserProducts)
    {
        _getUserProducts = getUserProducts;
    }

    [HttpGet("products", Name = "GetUserProducts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
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
}