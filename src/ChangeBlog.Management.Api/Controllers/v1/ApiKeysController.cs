using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Users.AddApiKey;
using ChangeBlog.Application.UseCases.Users.DeleteApiKey;
using ChangeBlog.Application.UseCases.Users.GetApiKeys;
using ChangeBlog.Application.UseCases.Users.UpdateApiKey;
using ChangeBlog.Management.Api.DTOs.V1.ApiKey;
using ChangeBlog.Management.Api.Presenters.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Controllers.v1;

[ApiController]
[Route("api/v1/user/apikeys")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[SwaggerControllerOrder(4)]
public class ApiKeysController : ControllerBase
{
    [HttpGet(Name = "GetApiKeys")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SkipAuthorization]
    public async Task<ActionResult<IEnumerable<ApiKeyDto>>> GetApiKeysAsync(
        [FromServices] IGetApiKeys getApiKeys)
    {
        var userId = HttpContext.GetUserId();
        var apiKeys = await getApiKeys.ExecuteAsync(userId);
        var dto = apiKeys.Select(ApiKeyDto.FromResponseModel);

        return Ok(dto);
    }

    [HttpPost(Name = "GenerateApiKey")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [SkipAuthorization]
    public async Task<ActionResult<SuccessResponse>> GenerateApiKeyAsync(
        [FromServices] IAddApiKey addApiKey,
        [FromBody] CreateOrUpdateApiKeyDto createApiKeyDto)
    {
        var userId = HttpContext.GetUserId();
        var presenter = new AddApiKeyApiPresenter();
        var expiresAt = createApiKeyDto.ExpiresAt!.Value;
        var requestModel = new AddApiKeyRequestModel(userId, createApiKeyDto.Name, expiresAt);

        await addApiKey.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpPatch("{apiKeyId:Guid}", Name = "UpdateApiKey")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [SkipAuthorization]
    public async Task<ActionResult<SuccessResponse>> UpdateApiKeyAsync(
        [FromServices] IUpdateApiKey updateApiKey,
        Guid apiKeyId,
        [FromBody] CreateOrUpdateApiKeyDto updateApiKeyDto)
    {
        var presenter = new UpdateApiKeyApiPresenter();
        var requestModel = new UpdateApiKeyRequestModel(apiKeyId,
            updateApiKeyDto.Name,
            updateApiKeyDto.ExpiresAt);

        await updateApiKey.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpDelete("{apiKeyId:Guid}", Name = "DeleteApiKey")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SkipAuthorization]
    public async Task<ActionResult<SuccessResponse>> DeleteApiKeyAsync(
        [FromServices] IDeleteApiKey deleteApiKey,
        Guid apiKeyId)
    {
        var userId = HttpContext.GetUserId();
        await deleteApiKey.ExecuteAsync(userId, apiKeyId);

        return Ok(SuccessResponse.Create(ChangeBlogStrings.ApiKeyDeleted));
    }
}