using System;
using System.Collections.Generic;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.UseCases.Commands.ReleaseVersion;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.Version
{
    public class ReleaseVersionApiPresenter : BaseApiPresenter, IReleaseVersionOutputPort
    {
        public void VersionAlreadyDeleted(Guid versionId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [KnownIdentifiers.VersionId] = versionId.ToString()
            };

            Response = new ConflictObjectResult(DefaultResponse.Create("The version has been deleted.", resourceIds));
        }

        public void VersionAlreadyReleased(Guid versionId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [KnownIdentifiers.VersionId] = versionId.ToString()
            };

            Response = new ConflictObjectResult(
                DefaultResponse.Create("The version has already been released.", resourceIds));
        }

        public void VersionReleased(Guid versionId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [KnownIdentifiers.VersionId] = versionId.ToString()
            };

            Response = new OkObjectResult(DefaultResponse.Create("Version successfully released.", resourceIds));
        }

        public void Conflict(Conflict conflict)
        {
            Response = conflict.ToResponse();
        }

        public void VersionDoesNotExist(Guid versionId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [KnownIdentifiers.VersionId] = versionId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("Version not found", resourceIds));
        }

        public void RelatedProductClosed(Guid productId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [KnownIdentifiers.ProductId] = productId.ToString()
            };

            Response = new ConflictObjectResult(
                DefaultResponse.Create("The related product has been closed.", resourceIds));
        }
    }
}
