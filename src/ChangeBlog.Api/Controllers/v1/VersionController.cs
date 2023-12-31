using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.DTOs.V1.Version;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Presenters.V1.Version;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion;
using ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion.Models;
using ChangeBlog.Application.UseCases.Versions.DeleteVersion;
using ChangeBlog.Application.UseCases.Versions.GetLatestVersion;
using ChangeBlog.Application.UseCases.Versions.GetVersions;
using ChangeBlog.Application.UseCases.Versions.ReleaseVersion;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Controllers.v1;

[ApiController]
[Route("api/v1")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[SwaggerControllerOrder(2)]
public class VersionController : ControllerBase
{
    private readonly IGetVersions _getVersions;

    public VersionController(IGetVersions getVersions)
    {
        _getVersions = getVersions;
    }

    [HttpGet("versions/{versionId:Guid}", Name = "GetVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [NeedsPermission(Permission.ViewVersions)]
    public async Task<ActionResult<VersionDto>> GetVersionAsync(
        Guid versionId,
        [FromServices] IGetVersion getVersion)
    {
        var userId = HttpContext.GetUserId();
        var version = await getVersion.ExecuteAsync(userId, versionId);

        if (version.HasNoValue)
            return new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.VersionNotFound));

        return Ok(VersionDto.FromResponseModel(version.GetValueOrThrow()));
    }

    [HttpGet("products/{productId:Guid}/versions", Name = "GerVersions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [NeedsPermission(Permission.ViewVersions)]
    public async Task<ActionResult<List<VersionDto>>> GetProductVersionsAsync(Guid productId,
        [MaxLength(VersionsQueryRequestModel.MaxSearchTermLength)]
        string searchTerm = null,
        Guid? lastVersionId = null,
        bool includeDeleted = false,
        [Range(1, VersionsQueryRequestModel.MaxLimit)]
        ushort limit = VersionsQueryRequestModel.MaxLimit)
    {
        var userId = HttpContext.GetUserId();
        var requestModel = new VersionsQueryRequestModel(productId,
            lastVersionId,
            userId,
            searchTerm,
            limit,
            includeDeleted);

        var versions = await _getVersions.ExecuteAsync(requestModel);

        return Ok(versions.Select(VersionDto.FromResponseModel));
    }

    [HttpGet("products/{productId:Guid}/versions/latest", Name = "GetLatestVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [NeedsPermission(Permission.ViewVersions)]
    public async Task<ActionResult<VersionDto>> GetLatestProductVersionAsync(
        [FromServices] IGetLatestVersion getLatestVersion,
        Guid productId)
    {
        var userId = HttpContext.GetUserId();
        var presenter = new GetLatestVersionApiPresenter();
        await getLatestVersion.ExecuteAsync(presenter, userId, productId);

        return presenter.Response;
    }

    [HttpGet("products/{productId:Guid}/versions/{version}", Name = "GetProductVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [NeedsPermission(Permission.ViewVersions)]
    public async Task<ActionResult<List<VersionDto>>> GetProductVersionAsync(
        [FromServices] IGetVersion getVersion,
        Guid productId,
        [Required] string version)
    {
        var userId = HttpContext.GetUserId();
        var clVersion = await getVersion.ExecuteAsync(userId, productId, version);

        if (clVersion.HasNoValue)
            return new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.VersionNotFound));

        return Ok(VersionDto.FromResponseModel(clVersion.GetValueOrThrow()));
    }

    [HttpPost("products/{productId:Guid}/versions", Name = "AddVersion")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.AddOrUpdateVersion)]
    public async Task<ActionResult<SuccessResponse>> AddVersionAsync([FromServices] IAddVersion addVersion,
        Guid productId,
        [FromBody] AddVersionDto versionDto)
    {
        if (versionDto is null)
            return new BadRequestObjectResult(ErrorResponse.Create(ChangeBlogStrings.MissingRequestData));

        var lines = versionDto.ChangeLogLines
            .Select(x =>
                new ChangeLogLineRequestModel(x.Text,
                    x.Labels ?? new List<string>(0),
                    x.Issues ?? new List<string>(0)))
            .ToList();

        var userId = HttpContext.GetUserId();
        var requestModel = new VersionRequestModel(userId, productId,
            versionDto.Version, versionDto.Name, lines, versionDto.ReleaseImmediately);

        var presenter = new AddOrUpdateVersionApiPresenter(HttpContext);
        await addVersion.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpPost("versions/{versionId:Guid}/release", Name = "ReleaseVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [NeedsPermission(Permission.ReleaseVersion)]
    public async Task<ActionResult<SuccessResponse>> ReleaseVersionAsync([FromServices] IReleaseVersion releaseVersion,
        Guid versionId)
    {
        var presenter = new ReleaseVersionApiPresenter();
        await releaseVersion.ExecuteAsync(presenter, versionId);

        return presenter.Response;
    }

    [HttpPut("products/{productId:Guid}/versions/{version}", Name = "UpdateVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.AddOrUpdateVersion)]
    public async Task<ActionResult<SuccessResponse>> AddOrUpdateVersionAsync(
        [FromServices] IAddOrUpdateVersion addOrUpdateVersion,
        Guid productId,
        string version,
        [FromBody] AddOrUpdateVersionDto addOrUpdateVersionDto)
    {
        var userId = HttpContext.GetUserId();

        var lines = addOrUpdateVersionDto.ChangeLogLines
            .Select(x =>
                new ChangeLogLineRequestModel(x.Text,
                    x.Labels ?? new List<string>(0),
                    x.Issues ?? new List<string>(0)))
            .ToList();

        var updateVersionRequestModel =
            new VersionRequestModel(userId,
                productId,
                version,
                addOrUpdateVersionDto.Name,
                lines,
                addOrUpdateVersionDto.ReleaseImmediately);

        var presenter = new AddOrUpdateVersionApiPresenter(HttpContext);
        await addOrUpdateVersion.ExecuteAsync(presenter, updateVersionRequestModel);

        return presenter.Response;
    }

    [HttpDelete("versions/{versionId:Guid}", Name = "DeleteVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [NeedsPermission(Permission.DeleteVersion)]
    public async Task<ActionResult<SuccessResponse>> DeleteVersionAsync([FromServices] IDeleteVersion deleteVersion,
        Guid versionId)
    {
        var presenter = new DeleteVersionApiPresenter();
        await deleteVersion.ExecuteAsync(presenter, versionId);

        return presenter.Response;
    }
}