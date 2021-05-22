using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.V1.Product;
using ChangeTracker.Api.DTOs.V1.Version;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.Presenters.V1.Product;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.AddProduct;
using ChangeTracker.Application.UseCases.Commands.CloseProduct;
using ChangeTracker.Application.UseCases.Queries.GetCompleteVersions;
using ChangeTracker.Application.UseCases.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IAddProduct _addProduct;
        private readonly ICloseProduct _closeProduct;
        private readonly IGetProduct _getProduct;
        private readonly IGetCompleteVersions _getCompleteVersions;

        public ProductController(IAddProduct addProduct, ICloseProduct closeProduct, IGetProduct getProduct,
            IGetCompleteVersions getCompleteVersions)
        {
            _addProduct = addProduct;
            _closeProduct = closeProduct;
            _getProduct = getProduct;
            _getCompleteVersions = getCompleteVersions;
        }

        [HttpGet("{productId:Guid}")]
        [NeedsPermission(Permission.ViewAccountProducts)]
        public async Task<ActionResult> GetProductAsync(Guid productId)
        {
            var userId = HttpContext.GetUserId();
            var product = await _getProduct.ExecuteAsync(userId, productId);

            if (product.HasNoValue) return NotFound(DefaultResponse.Create("Product not found"));

            return Ok(ProductDto.FromResponseModel(product.Value));
        }

        [HttpPost]
        [NeedsPermission(Permission.AddProduct)]
        public async Task<ActionResult> AddProductAsync([FromBody] AddProjectDto addProductDto)
        {
            if (addProductDto.VersioningSchemeId == Guid.Empty)
                return BadRequest(DefaultResponse.Create("VersioningSchemeId cannot be empty."));

            var presenter = new AddProductApiPresenter(HttpContext);
            var userId = HttpContext.GetUserId();

            var requestModel = new ProductRequestModel(addProductDto.AccountId, addProductDto.Name,
                addProductDto.VersioningSchemeId, userId);

            await _addProduct.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpPost("{productId:Guid}/close")]
        [NeedsPermission(Permission.CloseProduct)]
        public async Task<ActionResult> CloseProductAsync(Guid productId)
        {
            if (productId == Guid.Empty)
                return BadRequest(DefaultResponse.Create("Missing productId."));

            var presenter = new CloseProductApiPresenter();
            await _closeProduct.ExecuteAsync(presenter, productId);

            return presenter.Response;
        }

        [HttpGet("{productId:Guid}/versions")]
        [NeedsPermission(Permission.ViewCompleteVersion)]
        public async Task<ActionResult<CompleteVersionDto>> GetCompleteProductVersionsAsync(Guid productId,
            string searchTerm = null,
            Guid? lastVersionId = null,
            bool includeDeleted = false,
            ushort limit = VersionsQueryRequestModel.MaxLimit)
        {
            if (searchTerm is not null && searchTerm.Trim().Contains(" "))
                return BadRequest(DefaultResponse.Create("Whitespaces in search terms are not allowed."));

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
    }
}