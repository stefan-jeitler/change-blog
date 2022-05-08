using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.AssignPendingLineToVersion;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class AssignPendingLineToVersionApiPresenter : BaseApiPresenter,
    IAssignPendingLineToVersionOutputPort
{
    public void InvalidVersionFormat(string version)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.InvalidVersionFormat, version)));
    }

    public void VersionDoesNotExist()
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.VersionNotFound));
    }

    public void MaxChangeLogLinesReached(int maxChangeLogLines)
    {
        var message = string.Format(ChangeBlogStrings.TargetVersionTooManyChangeLogLines, maxChangeLogLines);
        
        Response = new ConflictObjectResult(
            ErrorResponse.Create(message));
    }

    public void ChangeLogLineDoesNotExist(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.ChangeLogLineNotFound, resourceIds));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void Assigned(Guid versionId, Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString(),
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.ChangeLogLineAdded, resourceIds));
    }

    public void ChangeLogLineIsNotPending(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new ConflictObjectResult(ErrorResponse.Create(ChangeBlogStrings.ChnageLogLineNotPending,
            resourceIds));
    }

    public void LineWithSameTextAlreadyExists(string text)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.TargetVersionContainsLinesWithSameText, text)));
    }

    public void TargetVersionBelongsToDifferentProduct(Guid changeLogLineProductId, Guid targetVersionProductId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = targetVersionProductId.ToString()
        };

        Response = new ConflictObjectResult(ErrorResponse.Create(ChangeBlogStrings.TargetVersionBelongsToDifferentProduct,resourceIds));
    }
}