using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.V1.Product;
using ChangeTracker.Api.DTOs.V1.Version;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.Presenters.V1.Product;
using ChangeTracker.Api.SwaggerUI;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.AddProduct;
using ChangeTracker.Application.UseCases.Commands.CloseProduct;
using ChangeTracker.Application.UseCases.Commands.SharedModels;
using ChangeTracker.Application.UseCases.Queries.GetCompleteVersions;
using ChangeTracker.Application.UseCases.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/products")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(3)]
    public class ProductController : ControllerBase
    {
        private readonly IGetCompleteVersions _getCompleteVersions;

        public ProductController(IGetCompleteVersions getCompleteVersions)
        {
            _getCompleteVersions = getCompleteVersions;
        }

        [HttpGet("{productId:Guid}")]
        [NeedsPermission(Permission.ViewAccountProducts)]
        public async Task<ActionResult<ProductDto>> GetProductAsync([FromServices] IGetProduct getProduct,
            Guid productId)
        {
            var userId = HttpContext.GetUserId();
            var product = await getProduct.ExecuteAsync(userId, productId);

            if (product.HasNoValue)
                return NotFound(DefaultResponse.Create("Product not found"));

            return Ok(ProductDto.FromResponseModel(product.Value));
        }

        [HttpGet("{productId:Guid}/versions")]
        [NeedsPermission(Permission.ViewCompleteVersions)]
        public async Task<ActionResult<List<CompleteVersionDto>>> GetCompleteProductVersionsAsync(Guid productId,
            [MaxLength(VersionsQueryRequestModel.MaxSearchTermLength)]
            string searchTerm = null,
            Guid? lastVersionId = null,
            bool includeDeleted = false,
            [Range(1, VersionsQueryRequestModel.MaxLimit)]
            ushort limit = VersionsQueryRequestModel.MaxLimit)
        {
            var userId = HttpContext.GetUserId();
            var requestModel = new VersionsQueryRequestModel(productId,
                lastVersionId,
                userId,
                searchTerm,
                limit,
                includeDeleted);

            var versions = await _getCompleteVersions.ExecuteAsync(requestModel);

            return Ok(versions.Select(CompleteVersionDto.FromResponseModel));
        }

        [HttpGet("{productId:Guid}/versions/{version}")]
        [NeedsPermission(Permission.ViewCompleteVersions)]
        public async Task<ActionResult<List<CompleteVersionDto>>> GetCompleteProductVersionAsync(
            [FromServices] IGetCompleteVersion getCompleteVersion,
            Guid productId,
            [Required] string version)
        {
            var userId = HttpContext.GetUserId();
            var completeVersion = await getCompleteVersion.ExecuteAsync(userId, productId, version);

            if (completeVersion.HasNoValue)
                return new NotFoundObjectResult(DefaultResponse.Create("Version not found"));

            return Ok(CompleteVersionDto.FromResponseModel(completeVersion.Value));
        }

        [HttpPost]
        [NeedsPermission(Permission.AddOrUpdateProduct)]
        public async Task<ActionResult> AddProductAsync([FromServices] IAddProduct addProduct,
            [FromBody] AddOrUpdateProductDto addOrUpdateProductDto)
        {
            if (addOrUpdateProductDto.VersioningSchemeId == Guid.Empty)
                return BadRequest(DefaultResponse.Create("VersioningSchemeId cannot be empty."));

            var presenter = new AddProductApiPresenter(HttpContext);
            var userId = HttpContext.GetUserId();

            var requestModel = new ProductRequestModel(addOrUpdateProductDto.AccountId, addOrUpdateProductDto.Name,
                addOrUpdateProductDto.VersioningSchemeId, userId);

            await addProduct.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpPost("{productId:Guid}/close")]
        [NeedsPermission(Permission.CloseProduct)]
        public async Task<ActionResult> CloseProductAsync([FromServices] ICloseProduct closeProduct,
            Guid productId)
        {
            if (productId == Guid.Empty)
                return BadRequest(DefaultResponse.Create("Missing productId."));

            var presenter = new CloseProductApiPresenter();
            await closeProduct.ExecuteAsync(presenter, productId);

            return presenter.Response;
        }
    }
}