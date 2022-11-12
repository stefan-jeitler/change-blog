using System;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Presenters.V1.Product;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.DTOs.V1.Product;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Products.AddProduct;
using ChangeBlog.Application.UseCases.Products.FreezeProduct;
using ChangeBlog.Application.UseCases.Products.GetProducts;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Controllers.v1;

[ApiController]
[Route("api/v1/products")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[SwaggerControllerOrder(1)]
public class ProductController : ControllerBase
{
    [HttpGet("{productId:Guid}", Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [NeedsPermission(Permission.ViewProduct)]
    public async Task<ActionResult<ProductDto>> GetProductAsync([FromServices] IGetProduct getProduct,
        Guid productId)
    {
        var userId = HttpContext.GetUserId();
        var product = await getProduct.ExecuteAsync(userId, productId);

        if (product.HasNoValue)
            return NotFound(ErrorResponse.Create("Product not found"));

        return Ok(ProductDto.FromResponseModel(product.GetValueOrThrow()));
    }

    [HttpPost(Name = "AddProduct")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.AddOrUpdateProduct)]
    public async Task<ActionResult<SuccessResponse>> AddProductAsync([FromServices] IAddProduct addProduct,
        [FromBody] AddOrUpdateProductDto addOrUpdateProductDto)
    {
        if (addOrUpdateProductDto.VersioningSchemeId == Guid.Empty)
            return BadRequest(ErrorResponse.Create(ChangeBlogStrings.InvalidVersioningSchemeId));

        var userId = HttpContext.GetUserId();

        var requestModel = new ProductRequestModel(addOrUpdateProductDto.AccountId,
            addOrUpdateProductDto.Name,
            addOrUpdateProductDto.VersioningSchemeId,
            addOrUpdateProductDto.LanguageCode,
            userId);

        var presenter = new AddProductApiPresenter(HttpContext);
        await addProduct.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpPost("{productId:Guid}/freeze", Name = "FreezeProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [NeedsPermission(Permission.FreezeProduct)]
    public async Task<ActionResult<SuccessResponse>> FreezeProductAsync([FromServices] IFreezeProduct freezeProduct,
        Guid productId)
    {
        var presenter = new FreezeProductApiPresenter();
        await freezeProduct.ExecuteAsync(presenter, productId);

        return presenter.Response;
    }
}