using System;
using System.Collections.Generic;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.UseCases.Commands.ReleaseVersion;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Version
{
    public class ReleaseVersionApiPresenter : BaseApiPresenter, IReleaseVersionOutputPort
    {
        public void VersionAlreadyDeleted(Guid versionId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(versionId)] = versionId.ToString()
            };

            Response = new OkObjectResult(DefaultResponse.Create("Version successfully released.", resourceIds));
        }

        public void VersionAlreadyReleased(Guid versionId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(versionId)] = versionId.ToString()
            };

            Response = new ConflictObjectResult(
                DefaultResponse.Create("The version has already been released.", resourceIds));
        }

        public void VersionReleased(Guid versionId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(versionId)] = versionId.ToString()
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
                [nameof(versionId)] = versionId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("Version not found", resourceIds));
        }

        public void RelatedProductClosed(Guid productId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(productId)] = productId.ToString()
            };

            Response = new ConflictObjectResult(
                DefaultResponse.Create("The related product has been closed.", resourceIds));
        }
    }
}