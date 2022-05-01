using System;
using System.Collections.Generic;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.UpdateUserProfile;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Presenters.V1;

public class UpdateUserProfileApiPresenter : BaseApiPresenter, IUpdateUserProfileOutputPort
{
    public void Updated(Guid userId)
    {
        Response = new OkObjectResult(DefaultResponse.Create("Userprofile successfully updated."));
    }

    public void TimezoneNotFound(string timezone)
    {
        Response = new NotFoundObjectResult(DefaultResponse.Create($"Unknown Timezone '{timezone}'"));
    }

    public void CultureNotFound(string culture)
    {
        Response = new NotFoundObjectResult(DefaultResponse.Create($"Unknown Culture '{culture}'"));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }
}