using System;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Authorization;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Api.DTOs.V1.Product;
using ChangeBlog.Api.Extensions;
using ChangeBlog.Api.Presenters.V1.Product;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Swagger;
using ChangeBlog.Application.UseCases;
using ChangeBlog.Application.UseCases.Commands.AddProduct;
using ChangeBlog.Application.UseCases.Commands.CloseProduct;
using ChangeBlog.Application.UseCases.Queries.GetProducts;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/products")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status403Forbidden)]
    [SwaggerControllerOrder(3)]
    public class ProductController : ControllerBase
    {
        [HttpGet("{productId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        [NeedsPermission(Permission.ViewAccountProducts)]
        public async Task<ActionResult<ProductDto>> GetProductAsync([FromServices] IGetProduct getProduct,
            Guid productId)
        {
            var userId = HttpContext.GetUserId();
            var product = await getProduct.ExecuteAsync(userId, productId);

            if (product.HasNoValue)
                return NotFound(DefaultResponse.Create("Product not found"));

            return Ok(ProductDto.FromResponseModel(product.GetValueOrThrow()));
        }

        [HttpPost]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status422UnprocessableEntity)]
        [NeedsPermission(Permission.AddOrUpdateProduct)]
        public async Task<ActionResult> AddProductAsync([FromServices] IAddProduct addProduct,
            [FromBody] AddOrUpdateProductDto addOrUpdateProductDto)
        {
            if (addOrUpdateProductDto.VersioningSchemeId == Guid.Empty)
                return BadRequest(DefaultResponse.Create("VersioningSchemeId cannot be empty."));

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

        [HttpPost("{productId:Guid}/close")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        [NeedsPermission(Permission.CloseProduct)]
        public async Task<ActionResult> CloseProductAsync([FromServices] ICloseProduct closeProduct,
            Guid productId)
        {
            var presenter = new CloseProductApiPresenter();
            await closeProduct.ExecuteAsync(presenter, productId);

            return presenter.Response;
        }
    }
}
