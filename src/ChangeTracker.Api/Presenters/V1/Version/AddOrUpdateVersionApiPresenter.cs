using System;
using System.Collections.Generic;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Extensions;
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
            Response = new ConflictObjectResult(
                DefaultResponse.Create("You cannot add or update versions that have been released.", versionId));
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

        public void ProductClosed(Guid productId)
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create("You cannot add or update a version when the related product has been closed."));
        }

        public void LinesWithSameTextsAreNotAllowed(IList<string> duplicates)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"Lines with the same texts are not allowed. Duplicates: {duplicates}"));
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

        public void VersionAlreadyExists(Guid versionId)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create($"Version already exists.", versionId));
        }

        public void TooManyLines(int maxChangeLogLines)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"Too many lines. Max lines: {maxChangeLogLines}"));
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