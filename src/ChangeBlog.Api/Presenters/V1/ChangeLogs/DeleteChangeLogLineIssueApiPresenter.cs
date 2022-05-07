using System;
using System.Collections.Generic;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.Issues.DeleteChangeLogLineIssue;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class DeleteChangeLogLineIssueApiPresenter : BaseApiPresenter, IDeleteChangeLogLineIssueOutputPort
{
    public void Removed(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create("Issue successfully deleted.", resourceIds));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void ChangeLogLineDoesNotExist()
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create("ChangeLogLine not found."));
    }

    public void InvalidIssue(string issue)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create($"Invalid issue '{issue}'."));
    }
}