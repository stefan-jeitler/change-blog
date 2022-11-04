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
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Controllers.v1;

[ApiController]
[Route("api/v1/accounts")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[SwaggerControllerOrder(1)]
public class AccountController : ControllerBase
{
    [HttpGet("{accountId:Guid}/products", Name = "GetAccountProducts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [NeedsPermission(Permission.ViewAccount)]
    public async Task<ActionResult<List<ProductDto>>> GetAccountProductsAsync(
        [FromServices] IGetAccountProducts getAccountProducts,
        Guid accountId,
        Guid? lastProductId = null,
        [Range(1, AccountProductQueryRequestModel.MaxLimit)]
        ushort limit = AccountProductQueryRequestModel.MaxLimit,
        bool includeFreezed = false)
    {
        var userId = HttpContext.GetUserId();
        var requestModel = new AccountProductQueryRequestModel(userId,
            accountId,
            lastProductId,
            limit,
            includeFreezed
        );

        var products = await getAccountProducts.ExecuteAsync(requestModel);

        return Ok(products.Select(ProductDto.FromResponseModel));
    }
}