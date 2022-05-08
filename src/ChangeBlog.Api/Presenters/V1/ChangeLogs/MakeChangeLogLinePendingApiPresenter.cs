using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.MakeChangeLogLinePending;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class MakeChangeLogLinePendingApiPresenter : BaseApiPresenter, IMakeChangeLogLinePendingOutputPort
{
    public void WasMadePending(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.ChangeLogLineMadePending, resourceIds));
    }

    public void ChangeLogLineDoesNotExist()
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.ChangeLogLineNotFound));
    }

    public void ChangeLogLineIsAlreadyPending(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.ChangeLogLineMadePending, resourceIds));
    }

    public void VersionAlreadyReleased(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new ConflictObjectResult(ErrorResponse.Create(ChangeBlogStrings.VersionAlreadyReleased,
            resourceIds));
    }

    public void VersionDeleted(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new ConflictObjectResult(ErrorResponse.Create(ChangeBlogStrings.VersionAlreadyDeleted,
            resourceIds));
    }

    public void TooManyPendingLines(int maxChangeLogLines)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.TooManyChangeLogLines, maxChangeLogLines)));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void LineWithSameTextAlreadyExists(Guid changeLogLineId, string duplicate)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.ChangeLogLineSameText, duplicate), resourceIds));
    }
}