using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.Issues.AddChangeLogLineIssue;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class AddChangeLogLineIssueApiPresenter : BaseApiPresenter, IAddChangeLogLineIssueOutputPort
{
    public void Added(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.IssueAdded, resourceIds));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void ChangeLogLineDoesNotExist()
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.ChangeLogLineNotFound));
    }

    public void InvalidIssue(string issue)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.InvalidIssue, issue)));
    }

    public void MaxIssuesReached(int maxIssues)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.MaxIssuesReached, maxIssues)));
    }
}