using System;
using System.Collections.Generic;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion;
using ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion.OutputPorts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Version
{
    public class AddVersionApiPresenter : BaseApiPresenter, IAddVersionOutputPort
    {
        private readonly HttpContext _httpContext;

        public AddVersionApiPresenter(HttpContext httpContext)
        {
            _httpContext = httpContext;
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

        public void ProductDoesNotExist(Guid productId)
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Product does not exist", productId));
        }

        public void ProductClosed(Guid productId)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create("You cannot add a version when the related product has been closed.", productId));
        }

        public void InvalidVersionFormat(string version)
        {
            Response = new UnprocessableEntityObjectResult(DefaultResponse.Create($"Invalid format '{version}'."));
        }

        public void VersionDoesNotMatchScheme(string version, string versioningSchemeName)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create(
                    $"Version does not match your product's versioning scheme. Version: {version}, Scheme: {versioningSchemeName}"));
        }

        public void Created(Guid versionId)
        {
            var location = _httpContext.CreateLinkTo($"api/v1/versions/{versionId}");
            Response = new CreatedResult(location, DefaultResponse.Create("Version added.", versionId));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }

        public void VersionAlreadyExists(string version)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create($"Version '{version}' already exists."));
        }

        public void TooManyLines(int maxChangeLogLines)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"Too many lines. Max lines: {maxChangeLogLines}"));
        }

        public void LinesWithSameTextsAreNotAllowed(IList<string> duplicates)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"Lines with the same texts are not allowed. Duplicates: {duplicates}"));
        }

        public void InvalidVersionName(string versionName)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"The name of the version is invalid. Name {versionName}"));
        }
    }
}