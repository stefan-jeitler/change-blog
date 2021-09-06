using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Authorization;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Api.DTOs.V1.ChangeLog;
using ChangeBlog.Api.Extensions;
using ChangeBlog.Api.Presenters.V1.ChangeLogs;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Swagger;
using ChangeBlog.Application.UseCases;
using ChangeBlog.Application.UseCases.Commands.AddChangeLogLine;
using ChangeBlog.Application.UseCases.Commands.AddChangeLogLine.Models;
using ChangeBlog.Application.UseCases.Commands.DeleteChangeLogLine;
using ChangeBlog.Application.UseCases.Commands.MakeAllChangeLogLinesPending;
using ChangeBlog.Application.UseCases.Commands.MakeChangeLogLinePending;
using ChangeBlog.Application.UseCases.Commands.UpdateChangeLogLine;
using ChangeBlog.Application.UseCases.Queries.GetChangeLogLine;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status403Forbidden)]
    [SwaggerControllerOrder(6)]
    public class ChangeLogsController : ControllerBase
    {
        [HttpGet("changelogs/{changeLogLineId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        [NeedsPermission(Permission.ViewChangeLogLines)]
        public async Task<ActionResult<ChangeLogLineDto>> GetChangeLogLineAsync(
            [FromServices] IGetChangeLogLine getChangeLogLine,
            Guid changeLogLineId)
        {
            var userId = HttpContext.GetUserId();

            var presenter = new GetChangeLogLineApiPresenter();
            await getChangeLogLine.ExecuteAsync(presenter, userId, changeLogLineId);

            return presenter.Response;
        }

        [HttpDelete("changelogs/{changeLogLineId:Guid}")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status409Conflict)]
        [NeedsPermission(Permission.DeleteChangeLogLine)]
        public async Task<ActionResult> DeleteChangeLogLineAsync(
            [FromServices] IDeleteChangeLogLine deleteChangeLogLine,
            Guid changeLogLineId)
        {
            var requestModel = new DeleteChangeLogLineRequestModel(changeLogLineId, ChangeLogLineType.NotPending);

            var presenter = new DeleteChangeLogLineApiPresenter();
            await deleteChangeLogLine.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpPost("versions/{versionId:Guid}/changelogs")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status422UnprocessableEntity)]
        [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
        public async Task<ActionResult> AddChangeLogLineAsync(
            [FromServices] IAddChangeLogLine addChangeLogLine,
            Guid versionId,
            [FromBody] AddOrUpdateChangeLogLineDto addChangeLogLineDto)
        {
            var userId = HttpContext.GetUserId();
            var requestModel = new VersionIdChangeLogLineRequestModel(userId,
                versionId,
                addChangeLogLineDto.Text,
                addChangeLogLineDto.Labels ?? new List<string>(0),
                addChangeLogLineDto.Issues ?? new List<string>(0));

            var presenter = new AddChangeLogLineApiPresenter(HttpContext);
            await addChangeLogLine.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpPatch("changelogs/{changeLogLineId:Guid}")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status422UnprocessableEntity)]
        [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
        public async Task<ActionResult> UpdateChangeLogLine(
            [FromServices] IUpdateChangeLogLine updateChangeLogLine,
            Guid changeLogLineId,
            [FromBody] PatchChangeLogLineDto patchChangeLogLineDto)
        {
            var requestModel = new UpdateChangeLogLineRequestModel(changeLogLineId,
                ChangeLogLineType.NotPending,
                patchChangeLogLineDto.Text,
                patchChangeLogLineDto.Labels,
                patchChangeLogLineDto.Issues);

            var presenter = new UpdateChangeLogLineApiPresenter();
            await updateChangeLogLine.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpPost("changelogs/{changeLogLineId:Guid}/make-pending")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status422UnprocessableEntity)]
        [NeedsPermission(Permission.MoveChangeLogLines)]
        public async Task<ActionResult> MakeChangeLogLinePendingAsync(
            [FromServices] IMakeChangeLogLinePending makeChangeLogLinePending,
            Guid changeLogLineId)
        {
            var presenter = new MakeChangeLogLinePendingApiPresenter();
            await makeChangeLogLinePending.ExecuteAsync(presenter, changeLogLineId);

            return presenter.Response;
        }

        [HttpPost("versions/{versionId:Guid}/changelogs/make-pending")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status422UnprocessableEntity)]
        [NeedsPermission(Permission.MoveChangeLogLines)]
        public async Task<ActionResult> MakeVersionChangeLogLinesPendingAsync(
            [FromServices] IMakeAllChangeLogLinesPending makeAllChangeLogLinesPending,
            Guid versionId)
        {
            var presenter = new MakeAllChangeLogLinesPendingApiPresenter();
            await makeAllChangeLogLinesPending.ExecuteAsync(presenter, versionId);

            return presenter.Response;
        }
    }
}
