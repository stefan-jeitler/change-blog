using System;
using System.Collections.Generic;
using ChangeBlog.Api.DTOs.V1.ChangeLog;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.UseCases.Queries.GetChangeLogLine;
using ChangeBlog.Application.UseCases.Queries.SharedModels;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class GetChangeLogLineApiPresenter : BaseApiPresenter, IGetChangeLogLineOutputPort
{
    public void LineDoesNotExists(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.ChangeLogLineNotFound, resourceIds));
    }

    public void LineIsPending(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.ChangeLogLinePending, resourceIds));
    }

    public void LineFound(ChangeLogLineResponseModel responseModel)
    {
        Response = new OkObjectResult(ChangeLogLineDto.FromResponseModel(responseModel));
    }
}