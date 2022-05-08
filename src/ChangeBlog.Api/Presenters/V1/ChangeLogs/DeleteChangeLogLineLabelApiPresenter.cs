using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
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

        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.LabelDeleted, resourceIds));
    }

    public void InvalidLabel(string label)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.InvalidLabel, label)));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void ChangeLogLineDoesNotExist()
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.ChangeLogLineNotFound));
    }
}