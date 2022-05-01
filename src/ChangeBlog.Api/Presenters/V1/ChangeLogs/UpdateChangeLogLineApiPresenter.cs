using System;
using System.Collections.Generic;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.UpdateChangeLogLine;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class UpdateChangeLogLineApiPresenter : BaseApiPresenter, IUpdateChangeLogLineOutputPort
{
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

    public void Updated(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new OkObjectResult(DefaultResponse.Create("ChangeLogLine successfully updated.", resourceIds));
    }

    public void ChangeLogLineDoesNotExist()
    {
        Response = new NotFoundObjectResult(DefaultResponse.Create("ChangeLogLine not found."));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void LineWithSameTextAlreadyExists(string text)
    {
        Response = new UnprocessableEntityObjectResult(
            DefaultResponse.Create($"Lines with same text are not allowed. Duplicate: '{text}'"));
    }

    public void RequestedLineIsNotPending(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new ConflictObjectResult(DefaultResponse.Create("The requested change log line is not pending.",
            resourceIds));
    }

    public void RequestedLineIsPending(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new ConflictObjectResult(DefaultResponse.Create("The requested change log line is pending.",
            resourceIds));
    }
}