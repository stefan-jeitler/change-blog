using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.DTOs.V1.User;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Queries.GetUsers;
using ChangeBlog.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using NodaTime.TimeZones;

namespace ChangeBlog.Management.Api.Controllers.v1;

[ApiController]
[Route("api/v1/user")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status403Forbidden)]
[SwaggerControllerOrder(1)]
public class UserController : ControllerBase
{
    private readonly IGetUsers _getUser;

    public UserController(IGetUsers getUser)
    {
        _getUser = getUser;
    }

    [HttpPost("import", Name = "EnsureUserIsImported")]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
    [SkipAuthorization]
    public ActionResult<DefaultResponse> EnsureUserIsImported()
    {
        return Ok(DefaultResponse.Create("It has now been ensured that the user is available in the app."));
    }
    
    [HttpGet("info", Name = "GetUserInfo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SkipAuthorization]
    public async Task<ActionResult<UserDto>> GetUserInfoAsync()
    {
        var userId = HttpContext.GetUserId();
        var user = await _getUser.ExecuteAsync(userId);

        return Ok(UserDto.FromResponseModel(user));
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