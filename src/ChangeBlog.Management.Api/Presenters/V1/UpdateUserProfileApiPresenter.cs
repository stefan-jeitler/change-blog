using System;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.UpdateUserProfile;
using ChangeBlog.Management.Api.DTOs.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ChangeBlog.Management.Api.Presenters.V1;

public class UpdateUserProfileApiPresenter : BaseApiPresenter, IUpdateUserProfileOutputPort
{
    private readonly IStringLocalizer<ChangeBlogStrings> _stringLocalizer;

    public UpdateUserProfileApiPresenter(IStringLocalizer<ChangeBlogStrings> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public void Updated(Guid userId)
    {
        var localizedMessage = _stringLocalizer.GetString(nameof(ChangeBlogStrings.UserProfileUpdated));
        Response = new OkObjectResult(SuccessResponse.Create(localizedMessage));
    }

    public void TimezoneNotFound(string timezone)
    {
        Response = new NotFoundObjectResult(
            ErrorResponse.Create($"Unknown Timezone '{timezone}'", nameof(UpdateUserProfileDto.Timezone)));
    }

    public void CultureNotFound(string culture)
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create($"Unknown Culture '{culture}'",
            nameof(UpdateUserProfileDto.Culture)));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }
}