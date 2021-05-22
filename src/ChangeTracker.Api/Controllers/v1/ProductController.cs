using System;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.V1.Product;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.Presenters.V1.Product;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.AddProduct;
using ChangeTracker.Application.UseCases.Commands.CloseProduct;
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

        public ProductController(IAddProduct addProduct, ICloseProduct closeProduct, IGetProduct getProduct)
        {
            _addProduct = addProduct;
            _closeProduct = closeProduct;
            _getProduct = getProduct;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="searchTerm">e.g. text, label or issue</param>
        /// <param name="lastVersionId"></param>
        /// <param name="includeDeleted"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("{productId:Guid}/versions")]
        [NeedsPermission(Permission.ViewCompleteVersion)]
        public async Task<ActionResult> GetProductVersionsAsync(Guid productId, 
            string searchTerm = null,
            Guid? lastVersionId = null,
            bool includeDeleted = false,
            ushort limit = 100)
        {
            await Task.Yield();

            return Ok();
        }
    }
}