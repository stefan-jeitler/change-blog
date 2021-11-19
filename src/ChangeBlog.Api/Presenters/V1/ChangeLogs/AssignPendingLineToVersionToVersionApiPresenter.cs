using System;
using System.Collections.Generic;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.UseCases.Commands.AssignPendingLineToVersion;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class AssignPendingLineToVersionToVersionApiPresenter : BaseApiPresenter,
    IAssignPendingLineToVersionOutputPort
{
    public void InvalidVersionFormat(string version)
    {
        Response = new UnprocessableEntityObjectResult(
            DefaultResponse.Create($"Invalid version format '{version}'"));
    }

    public void VersionDoesNotExist()
    {
        Response = new NotFoundObjectResult(DefaultResponse.Create("Version not found."));
    }

    public void MaxChangeLogLinesReached(int maxChangeLogLines)
    {
        Response = new ConflictObjectResult(
            DefaultResponse.Create(
                $"The target version has reached the max lines count. Max lines: {maxChangeLogLines}"));
    }

    public void ChangeLogLineDoesNotExist(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new NotFoundObjectResult(DefaultResponse.Create("ChangeLogLine not found.", resourceIds));
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

        Response = new OkObjectResult(DefaultResponse.Create("Line successfully moved.", resourceIds));
    }

    public void ChangeLogLineIsNotPending(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new ConflictObjectResult(DefaultResponse.Create("The given ChangeLogLine is not pending",
            resourceIds));
    }

    public void LineWithSameTextAlreadyExists(string text)
    {
        Response = new UnprocessableEntityObjectResult(
            DefaultResponse.Create(
                $"The target version contains already lines with an identical text. Duplicate: {text}"));
    }

    public void TargetVersionBelongsToDifferentProduct(Guid changeLogLineProductId, Guid targetVersionProductId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = targetVersionProductId.ToString()
        };

        Response = new ConflictObjectResult(DefaultResponse.Create("The target version belongs to a different product.", resourceIds));
    }
}