using System;
using System.Collections.Generic;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion.OutputPorts;
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
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(versionId)] = versionId.ToString()
            };

            Response = new ConflictObjectResult(
                DefaultResponse.Create("You cannot add or update versions that have been released.", resourceIds));
        }

        public void Created(Guid versionId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(versionId)] = versionId.ToString()
            };

            var location = _httpContext.CreateLinkTo($"api/v1/versions/{versionId}");
            Response = new CreatedResult(location, DefaultResponse.Create("Version added.", resourceIds));
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

        public void LinesWithSameTextsAreNotAllowed(IList<string> duplicates)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"Lines with same texts are not allowed. Duplicates: {duplicates}"));
        }

        public void InvalidVersionName(string name)
        {
            Response = new UnprocessableEntityObjectResult(DefaultResponse.Create($"Invalid name '{name}'."));
        }

        public void InsertConflict(Conflict conflict)
        {
            Response = conflict.ToResponse();
        }

        public void VersionUpdated(Guid versionId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(versionId)] = versionId.ToString()
            };

            Response = new OkObjectResult(DefaultResponse.Create("Version successfully updated.", resourceIds));
        }

        public void ProductDoesNotExist(Guid productId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(productId)] = productId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("Product does not exist", resourceIds));
        }

        public void VersionAlreadyExists(Guid versionId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(versionId)] = versionId.ToString()
            };

            Response = new ConflictObjectResult(DefaultResponse.Create("Version already exists.", resourceIds));
        }

        public void TooManyLines(int maxChangeLogLines)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"Too many lines. Max lines: {maxChangeLogLines}"));
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

        public void UpdateConflict(Conflict conflict)
        {
            Response = conflict.ToResponse();
        }

        public void VersionAlreadyDeleted(Guid versionId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(versionId)] = versionId.ToString()
            };

            Response = new ConflictObjectResult(
                DefaultResponse.Create("You cannot add or update versions that have been deleted.", resourceIds));
        }

        public void InvalidChangeLogLineText(string text)
        {
            Response = new BadRequestObjectResult(DefaultResponse.Create($"Invalid change log text '{text}'."));
        }

        public void InvalidIssue(string changeLogText, string issue)
        {
            Response = new BadRequestObjectResult(
                DefaultResponse.Create($"Invalid issue '{issue}' for change log '{changeLogText}'."));
        }

        public void TooManyIssues(string changeLogText, int maxIssues)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create(
                    $"The change log '{changeLogText}' has too many issues. Max issues: '{maxIssues}'."));
        }

        public void InvalidLabel(string changeLogText, string label)
        {
            Response = new BadRequestObjectResult(
                DefaultResponse.Create($"Invalid label '{label}' for change log '{changeLogText}'."));
        }

        public void TooManyLabels(string changeLogText, int maxLabels)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create(
                    $"The change log '{changeLogText}' has too many labels. Max labels: '{maxLabels}'."));
        }
    }
}