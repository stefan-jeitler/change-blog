using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases.Commands.AddProduct;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Product
{
    public class AddProductApiPresenter : BaseApiPresenter, IAddProductOutputPort
    {
        private readonly HttpContext _httpContext;

        public AddProductApiPresenter(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public void AccountDoesNotExist(Guid accountId)
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Account not found.", accountId));
        }

        public void AccountDeleted(Guid accountId)
        {
            Response = new UnprocessableEntityObjectResult(DefaultResponse.Create("The requested account has been deleted."));
        }

        public void InvalidName(string name)
        {
            Response = new BadRequestObjectResult(DefaultResponse.Create($"Invalid name {name}."));
        }

        public void ProductAlreadyExists(Guid productId)
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create($"Product already exists.", productId));
        }

        public void VersioningSchemeDoesNotExist(Guid versioningSchemeId)
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("VersioningScheme not found.", versioningSchemeId));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }

        public void Created(Guid accountId, Guid productId)
        {
            var location = _httpContext.CreateLinkTo($"api/v1/products/{productId}");
            Response = new CreatedResult(location, DefaultResponse.Create($"Product added.", productId));
        }
    }
}