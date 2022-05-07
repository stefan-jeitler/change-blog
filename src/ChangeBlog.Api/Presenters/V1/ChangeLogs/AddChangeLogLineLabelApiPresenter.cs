using System;
using System.Collections.Generic;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.Labels.AddChangeLogLineLabel;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class AddChangeLogLineLabelApiPresenter : BaseApiPresenter, IAddChangeLogLineLabelOutputPort
{
    public void Added(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create("Label successfully added.", resourceIds));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void ChangeLogLineDoesNotExist()
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create("ChangeLogLine not found."));
    }

    public void InvalidLabel(string label)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create($"Invalid label '{label}'."));
    }

    public void MaxLabelsReached(int maxLabels)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create($"Max labels count reached. Max labels: {maxLabels}"));
    }
}