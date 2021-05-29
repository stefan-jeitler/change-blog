using System;
using System.Collections.Generic;
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
using ChangeTracker.Application.UseCases.Commands.AddCompleteVersion;
using ChangeTracker.Application.UseCases.Commands.AddCompleteVersion.Models;
using ChangeTracker.Application.UseCases.Commands.DeleteVersion;
using ChangeTracker.Application.UseCases.Commands.ReleaseVersion;
using ChangeTracker.Application.UseCases.Commands.UpdateVersion;
using ChangeTracker.Application.UseCases.Queries.GetCompleteVersions;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/versions")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(4)]
    public class VersionController : ControllerBase
    {
        [HttpPost]
        [NeedsPermission(Permission.AddVersion)]
        public async Task<ActionResult> AddCompleteVersionAsync([FromServices] IAddCompleteVersion addCompleteVersion,
            [FromBody] AddCompleteVersionDto completeVersionDto)
        {
            if (completeVersionDto is null)
                return new BadRequestObjectResult(DefaultResponse.Create("Missing version dto."));

            var lines = completeVersionDto?
                .ChangeLogLines
                .Select(x =>
                    new ChangeLogLineRequestModel(x.Text,
                        x.Labels ?? new List<string>(0),
                        x.Issues ?? new List<string>(0)))
                .ToList();

            var userId = HttpContext.GetUserId();
            var requestModel = new CompleteVersionRequestModel(userId, completeVersionDto.ProductId,
                completeVersionDto.Version, completeVersionDto.Name, lines, completeVersionDto.ReleaseImmediately);

            var presenter = new AddCompleteVersionApiPresenter(HttpContext);
            await addCompleteVersion.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpGet("{versionId:Guid}")]
        [NeedsPermission(Permission.ViewCompleteVersion)]
        public async Task<ActionResult<CompleteVersionDto>> GetCompleteVersionAsync(
            [FromServices] IGetCompleteVersion getCompleteVersion,
            Guid versionId)
        {
            var userId = HttpContext.GetUserId();
            var completeVersion = await getCompleteVersion.ExecuteAsync(userId, versionId);

            if (completeVersion.HasNoValue)
                return new NotFoundObjectResult(DefaultResponse.Create("Version not found"));

            return Ok(CompleteVersionDto.FromResponseModel(completeVersion.Value));
        }

        [HttpPost("{versionId:Guid}/release")]
        [NeedsPermission(Permission.ReleaseVersion)]
        public async Task<ActionResult> ReleaseVersionAsync([FromServices] IReleaseVersion releaseVersion,
            Guid versionId)
        {
            var presenter = new ReleaseVersionPresenter();
            await releaseVersion.ExecuteAsync(presenter, versionId);

            return presenter.Response;
        }

        [HttpDelete("{versionId:Guid}")]
        [NeedsPermission(Permission.DeleteVersion)]
        public async Task<ActionResult> DeleteVersionAsync([FromServices] IDeleteVersion deleteVersion, Guid versionId)
        {
            var presenter = new DeleteVersionPresenter();
            await deleteVersion.ExecuteAsync(presenter, versionId);

            return presenter.Response;
        }

        [HttpPut("{versionId:Guid}")]
        [NeedsPermission(Permission.UpdateVersion)]
        public async Task<ActionResult> UpdateVersionAsync([FromServices] IUpdateVersion updateVersion,
            Guid versionId, [FromBody] UpdateVersionDto updateVersionDto)
        {
            var updateVersionRequestModel =
                new UpdateVersionRequestModel(versionId, updateVersionDto.Name, updateVersionDto.Version);

            var presenter = new UpdateVersionPresenter();
            await updateVersion.ExecuteAsync(presenter, updateVersionRequestModel);

            return presenter.Response;
        }
    }
}