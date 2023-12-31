using System;
using System.Collections.Generic;
using ChangeBlog.Api.DTOs.V1.ChangeLog;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.UseCases.ChangeLogs.GetPendingChangeLogLine;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class GetPendingChangeLogLineApiPresenter : BaseApiPresenter, IGetPendingChangeLogLineOutputPort
{
    public void LineDoesNotExist(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.ChangeLogLineNotFound, resourceIds));
    }

    public void LineIsNotPending(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.ChnageLogLineNotPending,
            resourceIds));
    }

    public void LineFound(PendingChangeLogLineResponseModel responseModel)
    {
        Response = new OkObjectResult(PendingChangeLogLineDto.FromResponseModel(responseModel));
    }
}