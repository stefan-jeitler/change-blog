using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.V1.Version;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.Presenters.V1.Version;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.AddCompleteVersion;
using ChangeTracker.Application.UseCases.Commands.AddCompleteVersion.Models;
using ChangeTracker.Application.UseCases.Queries.GetCompleteVersions;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/versions")]
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
        public async Task<ActionResult<CompleteVersionDto>> GetCompleteVersionAsync([FromServices] IGetCompleteVersion getCompleteVersion,
            Guid versionId)
        {
            var userId = HttpContext.GetUserId();
            var completeVersion = await getCompleteVersion.ExecuteAsync(userId, versionId);

            if (completeVersion.HasNoValue)
                return new NotFoundObjectResult(DefaultResponse.Create("Version not found"));

            return Ok(CompleteVersionDto.FromResponseModel(completeVersion.Value));
        }
    }
}