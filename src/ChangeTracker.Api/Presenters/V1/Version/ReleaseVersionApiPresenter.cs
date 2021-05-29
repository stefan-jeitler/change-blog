using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.ReleaseVersion;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Version
{
    public class ReleaseVersionApiPresenter : BaseApiPresenter, IReleaseVersionOutputPort
    {
        public void VersionAlreadyReleased()
        {
            Response = new NoContentResult();
        }

        public void VersionAlreadyDeleted()
        {
            Response = new ConflictObjectResult(DefaultResponse.Create("Version has been deleted."));
        }

        public void VersionReleased(Guid versionId)
        {
            Response = new NoContentResult();
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }

        public void VersionDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Version not found"));
        }

        public void RelatedProductClosed(Guid productId)
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create($"The related product has been closed. ProductId {productId}"));
        }
    }
}