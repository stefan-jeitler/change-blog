using System.Net.Mime;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
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
}