using System;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Users.UpdateUserProfile;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Presenters.V1;

public class UpdateUserProfileApiPresenter : BaseApiPresenter, IUpdateUserProfileOutputPort
{
    public void Updated(Guid userId)
    {
        var message = ChangeBlogStrings.UserProfileUpdated;

        Response = new OkObjectResult(SuccessResponse.Create(message));
    }

    public void TimezoneNotFound(string timezone)
    {
        Response = new NotFoundObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.TimezoneNotFound, timezone), "timezone"));
    }

    public void CultureNotFound(string culture)
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create(
            string.Format(ChangeBlogStrings.CultureNotSupported, culture),
            "culture"));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }
}