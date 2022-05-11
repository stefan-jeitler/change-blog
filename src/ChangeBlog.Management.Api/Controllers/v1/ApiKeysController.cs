using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.DTOs.V1.User;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Commands.AddApiKey;
using ChangeBlog.Application.UseCases.Queries.GetApiKeys;
using ChangeBlog.Management.Api.DTOs.V1;
using ChangeBlog.Management.Api.Presenters.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Controllers.v1;

[ApiController]
[Route("api/v1/user/apikeys")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[SwaggerControllerOrder(2)]
public class ApiKeysController : ControllerBase
{
    [HttpGet(Name = "GetUserApiKeys")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SkipAuthorization]
    public async Task<ActionResult<UserDto>> GetUserApiKeysAsync(
        [FromServices] IGetApiKeys getApiKeys)
    {
        var userId = HttpContext.GetUserId();
        var apiKeys = await getApiKeys.ExecuteAsync(userId);
        var dto = apiKeys.Select(ApiKeyDto.FromResponseModel);

        return Ok(dto);
    }

    [HttpPost(Name = "GenerateUserApiKey")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SkipAuthorization]
    public async Task<ActionResult<UserDto>> GenerateUserApiKeyAsync(
        [FromServices] IAddApiKey addApiKey,
        uint expiresInWeeks)
    {
        var userId = HttpContext.GetUserId();
        var presenter = new AddApiKeyPresenter();

        await addApiKey.ExecuteAsync(presenter, userId, TimeSpan.FromDays(expiresInWeeks * 7));

        return presenter.Response;
    }
}