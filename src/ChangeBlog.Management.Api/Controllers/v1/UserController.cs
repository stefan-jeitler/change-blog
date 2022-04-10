using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Controllers.v1;

[ApiController]
[Route("api/v1/user")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status403Forbidden)]
[SwaggerControllerOrder(1)]
public class UserController : ControllerBase
{
    [HttpPost("import", Name = "EnsureUserIsImported")]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
    [SkipAuthorization]
    public ActionResult<DefaultResponse> EnsureUserIsImported()
    {
        return Ok(DefaultResponse.Create("It has now been ensured that the user is available in the app."));
    }

    [HttpGet("photo", Name = "GetUserPhoto")]
    [ProducesResponseType(typeof(IFormFile), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
    [SkipAuthorization]
    public async Task<ActionResult<IFormFile>> GetUserPhotoAsync([FromServices] IExternalUserInfoDao externalUserInfo)
    {
        var (hasValue, userPhoto) = await externalUserInfo.GetUserPhotoAsync();

        return hasValue
            ? File(userPhoto.Photo, userPhoto.ContentType)
            : NotFound(DefaultResponse.Create("User Photo not found."));
    }
}