using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.V1.Version;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.Presenters.V1.Version;
using ChangeTracker.Api.SwaggerUI;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion;
using ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion.Models;
using ChangeTracker.Application.UseCases.Commands.DeleteVersion;
using ChangeTracker.Application.UseCases.Commands.ReleaseVersion;
using ChangeTracker.Application.UseCases.Queries.GetVersions;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(4)]
    public class VersionController : ControllerBase
    {
        private readonly IGetVersions _getVersions;

        public VersionController(IGetVersions getVersions)
        {
            _getVersions = getVersions;
        }

        [HttpGet("versions/{versionId:Guid}")]
        [NeedsPermission(Permission.ViewVersions)]
        public async Task<ActionResult<VersionDto>> GetVersionAsync(
            Guid versionId,
            [FromServices] IGetVersion getVersion)
        {
            var userId = HttpContext.GetUserId();
            var version = await getVersion.ExecuteAsync(userId, versionId);

            if (version.HasNoValue)
                return new NotFoundObjectResult(DefaultResponse.Create("Version not found"));

            return Ok(VersionDto.FromResponseModel(version.Value));
        }

        [HttpGet("products/{productId:Guid}/versions")]
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

        [HttpGet("products/{productId:Guid}/versions/{version}")]
        [NeedsPermission(Permission.ViewVersions)]
        public async Task<ActionResult<List<VersionDto>>> GetProductVersionAsync(
            [FromServices] IGetVersion getVersion,
            Guid productId,
            [Required] string version)
        {
            var userId = HttpContext.GetUserId();
            var clVersion = await getVersion.ExecuteAsync(userId, productId, version);

            if (clVersion.HasNoValue)
                return new NotFoundObjectResult(DefaultResponse.Create("Version not found"));

            return Ok(VersionDto.FromResponseModel(clVersion.Value));
        }

        [HttpPost("products/{productId:Guid}/versions")]
        [NeedsPermission(Permission.AddVersion)]
        public async Task<ActionResult> AddVersionAsync([FromServices] IAddVersion addVersion,
            Guid productId,
            [FromBody] AddVersionDto versionDto)
        {
            if (versionDto is null)
                return new BadRequestObjectResult(DefaultResponse.Create("Missing version dto."));

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

        [HttpPost("versions/{versionId:Guid}/release")]
        [NeedsPermission(Permission.ReleaseVersion)]
        public async Task<ActionResult> ReleaseVersionAsync([FromServices] IReleaseVersion releaseVersion,
            Guid versionId)
        {
            var presenter = new ReleaseVersionApiPresenter();
            await releaseVersion.ExecuteAsync(presenter, versionId);

            return presenter.Response;
        }

        [HttpPut("products/{productId:Guid}/versions/{version}")]
        [NeedsPermission(Permission.AddOrUpdateVersion)]
        public async Task<ActionResult> AddOrUpdateVersionAsync([FromServices] IAddOrUpdateVersion addOrUpdateVersion,
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

        [HttpDelete("versions/{versionId:Guid}")]
        [NeedsPermission(Permission.DeleteVersion)]
        public async Task<ActionResult> DeleteVersionAsync([FromServices] IDeleteVersion deleteVersion, Guid versionId)
        {
            var presenter = new DeleteVersionApiPresenter();
            await deleteVersion.ExecuteAsync(presenter, versionId);

            return presenter.Response;
        }
    }
}