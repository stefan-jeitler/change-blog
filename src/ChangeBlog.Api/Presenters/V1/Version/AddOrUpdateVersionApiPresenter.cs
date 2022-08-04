using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion.OutputPorts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.Version;

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
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new ConflictObjectResult(
            ErrorResponse.Create(ChangeBlogStrings.VersionUpdateForbidden, resourceIds));
    }

    public void Created(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        var location = _httpContext.CreateLinkTo($"api/v1/versions/{versionId}");
        Response = new CreatedResult(location, SuccessResponse.Create(ChangeBlogStrings.VersionAdded, resourceIds));
    }

    public void InvalidVersionFormat(string version)
    {
        Response = new UnprocessableEntityObjectResult(ErrorResponse.Create(string.Format(ChangeBlogStrings.InvalidVersionFormat, version)));
    }

    public void VersionDoesNotMatchScheme(string version, string versioningSchemeName)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.VersioningSchemeMismatch, version, versioningSchemeName)));
    }

    public void LinesWithSameTextsAreNotAllowed(IList<string> duplicates)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.ChangeLogLineSameText, string.Join(", ", duplicates))));
    }

    public void InvalidVersionName(string name)
    {
        Response = new UnprocessableEntityObjectResult(ErrorResponse.Create(string.Format(ChangeBlogStrings.InvalidName, name)));
    }

    public void InsertConflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void VersionUpdated(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.VersionUpdated, resourceIds));
    }

    public void ProductDoesNotExist(Guid productId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = productId.ToString()
        };

        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.ProductNotFound, resourceIds));
    }

    public void VersionAlreadyExists(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new ConflictObjectResult(ErrorResponse.Create(ChangeBlogStrings.VersionAlreadyExists, resourceIds));
    }

    public void TooManyLines(int maxChangeLogLines)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.TooManyChangeLogLines, maxChangeLogLines)));
    }

    public void RelatedProductClosed(Guid productId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = productId.ToString()
        };

        Response = new ConflictObjectResult(
            ErrorResponse.Create(ChangeBlogStrings.ProductAlreadyClosed, resourceIds));
    }

    public void UpdateConflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void VersionAlreadyDeleted(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new ConflictObjectResult(
            ErrorResponse.Create(ChangeBlogStrings.VersionUpdateForbidden, resourceIds));
    }

    public void InvalidChangeLogLineText(string text)
    {
        Response = new BadRequestObjectResult(ErrorResponse.Create(string.Format(ChangeBlogStrings.InvalidChangeLogText, text)));
    }

    public void InvalidIssue(string changeLogText, string issue)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.InvalidIssue, issue)));
    }

    public void TooManyIssues(string changeLogText, int maxIssues)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.TooManyIssuesForChangeLogLine, changeLogText, maxIssues)));
    }

    public void InvalidLabel(string changeLogText, string label)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.InvalidLabel, label)));
    }

    public void TooManyLabels(string changeLogText, int maxLabels)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.TooManyLabelsForChangeLogLine, changeLogText, maxLabels)));
    }
}