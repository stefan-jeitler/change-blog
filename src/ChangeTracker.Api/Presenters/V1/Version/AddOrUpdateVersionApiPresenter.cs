using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases.Commands.UpdateVersion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Version
{
    public class AddOrUpdateVersionApiPresenter : BaseApiPresenter, IAddOrUpdateVersionOutputPort
    {
        private readonly HttpContext _httpContext;

        public AddOrUpdateVersionApiPresenter(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public void VersionDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Version not found"));
        }

        public void VersionAlreadyDeleted()
        {
            Response = new ConflictObjectResult(DefaultResponse.Create("You cannot add or update versions that have been deleted."));
        }

        public void VersionAlreadyReleased()
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create("You cannot add or update versions that have been released."));
        }

        public void Created(Guid id)
        {
            var location = _httpContext.CreateLinkTo($"api/v1/versions/{id}");
            Response = new CreatedResult(location, DefaultResponse.Create("Version added.", id));
        }

        public void InvalidVersionFormat(string version)
        {
            Response = new UnprocessableEntityObjectResult(DefaultResponse.Create($"Invalid format '{version}'."));
        }

        public void ProductClosed()
        {
            Response = new ConflictObjectResult(DefaultResponse.Create("You cannot add or update a version when the related product has been closed."));
        }

        public void InvalidVersionName(string name)
        {
            Response = new UnprocessableEntityObjectResult(DefaultResponse.Create($"Invalid name '{name}'."));
        }

        public void VersionWithSameValueAlreadyExists(string value)
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create("A version with the same value already exists."));
        }

        public void VersionUpdated(Guid versionId)
        {
            Response = new OkObjectResult(DefaultResponse.Create("Version successfully updated.", versionId));
        }

        public void ProductDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Product does not exist"));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }

        public void VersionAlreadyExists(string version)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create($"Version '{version}' already exists."));
        }

        public void RelatedProductClosed(Guid productId)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create("Versions of closed products can no longer be modified."));
        }

        public void VersionDoesNotMatchScheme(string version)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create(
                    $"Version does not match your product's versioning scheme. Version '{version}'"));
        }
    }
}