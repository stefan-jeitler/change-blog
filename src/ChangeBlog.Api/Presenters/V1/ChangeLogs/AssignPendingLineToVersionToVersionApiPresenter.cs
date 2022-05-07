using System;
using System.Collections.Generic;
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
            ErrorResponse.Create($"Invalid version format '{version}'"));
    }

    public void VersionDoesNotExist()
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create("Version not found."));
    }

    public void MaxChangeLogLinesReached(int maxChangeLogLines)
    {
        Response = new ConflictObjectResult(
            ErrorResponse.Create(
                $"The target version has reached the max lines count. Max lines: {maxChangeLogLines}"));
    }

    public void ChangeLogLineDoesNotExist(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new NotFoundObjectResult(ErrorResponse.Create("ChangeLogLine not found.", resourceIds));
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

        Response = new OkObjectResult(SuccessResponse.Create("Line successfully moved.", resourceIds));
    }

    public void ChangeLogLineIsNotPending(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new ConflictObjectResult(ErrorResponse.Create("The given ChangeLogLine is not pending",
            resourceIds));
    }

    public void LineWithSameTextAlreadyExists(string text)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(
                $"The target version contains already lines with an identical text. Duplicate: {text}"));
    }

    public void TargetVersionBelongsToDifferentProduct(Guid changeLogLineProductId, Guid targetVersionProductId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = targetVersionProductId.ToString()
        };

        Response = new ConflictObjectResult(ErrorResponse.Create("The target version belongs to a different product.",
            resourceIds));
    }
}