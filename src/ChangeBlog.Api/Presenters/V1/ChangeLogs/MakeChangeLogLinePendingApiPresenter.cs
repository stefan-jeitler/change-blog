using System;
using System.Collections.Generic;
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

        Response = new OkObjectResult(DefaultResponse.Create("Line was made pending.", resourceIds));
    }

    public void ChangeLogLineDoesNotExist()
    {
        Response = new NotFoundObjectResult(DefaultResponse.Create("ChangeLogLine not found."));
    }

    public void ChangeLogLineIsAlreadyPending(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new OkObjectResult(DefaultResponse.Create("Line was made pending.", resourceIds));
    }

    public void VersionAlreadyReleased(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new ConflictObjectResult(DefaultResponse.Create("The related version has already been released.",
            resourceIds));
    }

    public void VersionDeleted(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new ConflictObjectResult(DefaultResponse.Create("The related version has been deleted.",
            resourceIds));
    }

    public void TooManyPendingLines(int maxChangeLogLines)
    {
        Response = new UnprocessableEntityObjectResult(
            DefaultResponse.Create($"Too many lines. Max lines: {maxChangeLogLines}"));
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
            DefaultResponse.Create($"Lines with same text are not allowed. Duplicate: '{duplicate}'", resourceIds));
    }
}