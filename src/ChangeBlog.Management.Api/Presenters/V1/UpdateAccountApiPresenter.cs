using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Accounts.UpdateAccount;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Presenters.V1;

public class UpdateAccountApiPresenter : BaseApiPresenter, IUpdateAccountOutputPort
{
    public void NewNameAlreadyTaken(string newAccountName)
    {
        Response = new UnprocessableEntityObjectResult(ErrorResponse.Create(ChangeBlogStrings.NameAlreadyTaken,
            "Name"));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void Updated(Guid accountId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.AccountId] = accountId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.AccountUpdated, resourceIds));
    }
}