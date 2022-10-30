using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Accounts.DeleteAccount;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Presenters.V1;

public class DeleteAccountApiPresenter : BaseApiPresenter, IDeleteAccountOutputPort
{
    public void AccountDeleted(Guid accountId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.AccountId] = accountId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.AccountDeleted, resourceIds));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }
}