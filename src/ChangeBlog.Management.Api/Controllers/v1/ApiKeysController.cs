using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.DTOs.V1.User;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Commands.AddApiKey;
using ChangeBlog.Application.UseCases.Commands.DeleteApiKey;
using ChangeBlog.Application.UseCases.Commands.UpdateApiKey;
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
        string title,
        [Required]
        DateTime expiresAt)
    {
        var userId = HttpContext.GetUserId();
        var presenter = new AddApiKeyPresenter();
        var requestModel = new AddApiKeyRequestModel(userId, title, expiresAt);

        await addApiKey.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpPatch("{apiKeyId:Guid}", Name = "UpdateApiKey")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [SkipAuthorization]
    public async Task<ActionResult<SuccessResponse>> UpdateApiKeyAsync(
        [FromServices] IUpdateApiKey updateApiKey,
        Guid apiKeyId,
        string title,
        DateTime? expiresAt)
    {
        var presenter = new UpdateApiKeyPresenter();
        var requestModel = new UpdateApiKeyRequestModel(apiKeyId, title, expiresAt);

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