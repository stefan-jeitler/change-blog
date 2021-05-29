using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.DeleteVersion;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Version
{
    public class DeleteVersionApiPresenter : BaseApiPresenter, IDeleteVersionOutputPort
    {
        public void VersionDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Version not found"));
        }

        public void RelatedProductClosed(Guid productId)
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create($"The related product has been closed. ProductId {productId}"));
        }

        public void VersionAlreadyDeleted()
        {
            Response = new NoContentResult();
        }

        public void VersionAlreadyReleased()
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create("Version released. Released versions can no longer be modified."));
        }

        public void VersionDeleted(Guid versionId)
        {
            Response = new NoContentResult();
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }
    }
}