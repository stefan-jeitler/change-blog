using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion;
using ChangeTracker.Domain.Common;
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

        public void VersionAlreadyReleased(Guid versionId)
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create("You cannot add or update versions that have been released.,", versionId));
        }

        public void Created(Guid versionId)
        {
            var location = _httpContext.CreateLinkTo($"api/v1/versions/{versionId}");
            Response = new CreatedResult(location, DefaultResponse.Create("Version added.", versionId));
        }

        public void InvalidVersionFormat(string version)
        {
            Response = new UnprocessableEntityObjectResult(DefaultResponse.Create($"Invalid format '{version}'."));
        }

        public void VersionDoesNotMatchScheme(string version, string versioningSchemeName)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create(
                    $"Version does not match your product's versioning scheme. Version '{version}', Scheme: {versioningSchemeName}"));
        }

        public void VersionAlreadyExists(Guid versionId)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create($"Version already exists.", versionId));
        }

        public void ProductClosed(Guid productId)
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create("You cannot add or update a version when the related product has been closed."));
        }

        public void InvalidVersionName(string name)
        {
            Response = new UnprocessableEntityObjectResult(DefaultResponse.Create($"Invalid name '{name}'."));
        }
        public void VersionUpdated(Guid versionId)
        {
            Response = new OkObjectResult(DefaultResponse.Create("Version successfully updated.", versionId));
        }

        public void ProductDoesNotExist(Guid productId)
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Product does not exist", productId));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }

        public void RelatedProductClosed(Guid productId)
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create("Versions of closed products can no longer be modified.", productId));
        }

        public void VersionAlreadyDeleted(Guid versionId)
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create("You cannot add or update versions that have been deleted.", versionId));
        }
    }
}