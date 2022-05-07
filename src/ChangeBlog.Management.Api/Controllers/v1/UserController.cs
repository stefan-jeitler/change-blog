﻿using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.DTOs.V1;
using ChangeBlog.Api.Shared.DTOs.V1.User;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Commands.UpdateUserProfile;
using ChangeBlog.Application.UseCases.Queries.GetUsers;
using ChangeBlog.Domain;
using ChangeBlog.Management.Api.DTOs.V1;
using ChangeBlog.Management.Api.Presenters.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NodaTime;
using NodaTime.TimeZones;

namespace ChangeBlog.Management.Api.Controllers.v1;

[ApiController]
[Route("api/v1/user")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[SwaggerControllerOrder(1)]
public class UserController : ControllerBase
{
    private readonly IGetUsers _getUser;
    private readonly IStringLocalizer<ChangeBlogStrings> _stringLocalizer;

    public UserController(IGetUsers getUser, IStringLocalizer<ChangeBlogStrings> stringLocalizer)
    {
        _getUser = getUser;
        _stringLocalizer = stringLocalizer;
    }

    [HttpPost("import", Name = "EnsureUserIsImported")]
    [ProducesResponseType( StatusCodes.Status200OK)]
    [SkipAuthorization]
    public ActionResult<SuccessResponse> EnsureUserIsImported()
    {
        return Ok(ErrorResponse.Create("It has now been ensured that the user is available in the app."));
    }

    [HttpGet("profile", Name = "GetUserProfile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SkipAuthorization]
    public async Task<ActionResult<UserDto>> GetUserProfileAsync()
    {
        var userId = HttpContext.GetUserId();
        var user = await _getUser.ExecuteAsync(userId);

        return Ok(UserDto.FromResponseModel(user));
    }

    [HttpPatch("profile", Name = "UpdateUserProfile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [SkipAuthorization]
    public async Task<ActionResult<SuccessResponse>> UpdateUserProfile(
        [FromServices] IUpdateUserProfile updateUserProfile,
        [FromServices] UpdateUserProfileApiPresenter presenter,
        [FromBody] UpdateUserProfileDto updateUserProfileDto)
    {
        var userId = HttpContext.GetUserId();
        var requestModel =
            new UpdateUserProfileRequestModel(userId, updateUserProfileDto?.Timezone, updateUserProfileDto?.Culture);
        await updateUserProfile.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }
    
    
    [HttpGet("culture", Name = "GetUserCulture")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SkipAuthorization]
    public async Task<ActionResult<CultureDto>> GetUserCultureAsync()
    {
        var userId = HttpContext.GetUserId();
        var user = await _getUser.ExecuteAsync(userId);

        var cultureInfo = CultureInfo.GetCultureInfo(user.Culture);
        var regionInfo = new RegionInfo(cultureInfo.Name);
        
        var cultureDto = new CultureDto(user.Culture,
            cultureInfo.TwoLetterISOLanguageName,
            regionInfo.TwoLetterISORegionName);

        return Ok(cultureDto);
    }

    [HttpGet("supported-cultures", Name = "GetSupportedCultures")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SkipAuthorization]
    public ActionResult<string[]> GetSupportedCultures()
    {
        var supportedCultures = Constants.SupportedCultures
            .Select(x => x.Value);

        return Ok(supportedCultures);
    }

    [HttpGet("supported-timezones", Name = "GetSupportedTimezones")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SkipAuthorization]
    public ActionResult<TimezoneDto[]> GetSupportedTimezones()
    {
        var supportedTimezones = TzdbDateTimeZoneSource.Default
            .WindowsToTzdbIds
            .Select(x =>
            {
                var now = SystemClock.Instance.GetCurrentInstant();
                var offset = DateTimeZoneProviders.Tzdb[x.Value].GetUtcOffset(now);
                var offsetFormatted = offset.ToString("m", CultureInfo.InvariantCulture);
                return new TimezoneDto(x.Key, x.Value, offsetFormatted);
            });

        return Ok(supportedTimezones);
    }
}