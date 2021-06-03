using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.DeleteVersion;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Version
{
    public class DeleteVersionApiPresenter : BaseApiPresenter, IDeleteVersionOutputPort
    {
        public void VersionDoesNotExist(Guid versionId)
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Version not found.", versionId));
        }

        public void RelatedProductClosed(Guid productId)
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create("The related product has been closed.", productId));
        }

        public void VersionAlreadyDeleted(Guid versionId)
        {
            Response = new OkObjectResult(DefaultResponse.Create("Version deleted.", versionId));
        }

        public void VersionAlreadyReleased(Guid versionId)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create("Version released. Released versions can no longer be modified.", versionId));
        }

        public void VersionDeleted(Guid versionId)
        {
            Response = new OkObjectResult(DefaultResponse.Create("Version deleted.", versionId));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }
    }
}