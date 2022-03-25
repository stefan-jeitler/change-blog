using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Queries.GetUsers;
using ChangeBlog.Management.Api.DTOs;
using ChangeBlog.Management.Api.Extensions;
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
    [HttpGet("info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> GetUserInfoAsync([FromServices] IGetUsers getUser)
    {
        var userId = HttpContext.GetUserId();
        var user = await getUser.ExecuteAsync(userId);

        return Ok(UserDto.FromResponseModel(user));
    }
}