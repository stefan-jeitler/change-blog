using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.ReleaseVersion;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Version
{
    public class ReleaseVersionApiPresenter : BaseApiPresenter, IReleaseVersionOutputPort
    {
        public void VersionAlreadyDeleted(Guid versionId)
        {
            Response = new OkObjectResult(DefaultResponse.Create("Version deleted.", versionId));
        }

        public void VersionAlreadyReleased(Guid versionId)
        {
            Response = new OkObjectResult(DefaultResponse.Create("Version released.", versionId));
        }

        public void VersionReleased(Guid versionId)
        {
            Response = new OkObjectResult(DefaultResponse.Create("Version released.", versionId));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }

        public void VersionDoesNotExist(Guid versionId)
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Version not found", versionId));
        }

        public void RelatedProductClosed(Guid productId)
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create($"The related product has been closed. ProductId {productId}", productId));
        }
    }
}