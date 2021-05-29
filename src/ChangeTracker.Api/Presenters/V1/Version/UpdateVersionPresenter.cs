using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.UpdateVersion;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Version
{
    public class UpdateVersionPresenter : BasePresenter, IUpdateVersionOutputPort
    {
        public void VersionDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Version not found"));
        }

        public void VersionAlreadyDeleted()
        {
            Response = new ConflictObjectResult(DefaultResponse.Create("You cannot update versions that have been deleted."));
        }

        public void VersionAlreadyReleased()
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create("You cannot release update version that have been released."));
        }

        public void InvalidVersionFormat(string version)
        {
            Response = new UnprocessableEntityObjectResult(DefaultResponse.Create($"Invalid format '{version}'."));
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
            Response = new NoContentResult();
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
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