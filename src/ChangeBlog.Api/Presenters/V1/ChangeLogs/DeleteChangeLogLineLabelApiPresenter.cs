using System;
using System.Collections.Generic;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.UseCases.Commands.Labels.DeleteChangeLogLineLabel;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class DeleteChangeLogLineLabelApiPresenter : BaseApiPresenter, IDeleteChangeLogLineLabelOutputPort
{
    public void Deleted(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new OkObjectResult(DefaultResponse.Create("Label successfully deleted.", resourceIds));
    }

    public void InvalidLabel(string label)
    {
        Response = new BadRequestObjectResult(
            DefaultResponse.Create($"Invalid label '{label}'."));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void ChangeLogLineDoesNotExist()
    {
        Response = new NotFoundObjectResult(DefaultResponse.Create("ChangeLogLine not found."));
    }
}