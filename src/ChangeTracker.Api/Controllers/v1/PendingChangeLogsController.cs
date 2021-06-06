using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.V1.ChangeLog;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.Presenters.V1.ChangeLogs;
using ChangeTracker.Api.SwaggerUI;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.AddPendingChangeLogLine;
using ChangeTracker.Application.UseCases.Commands.AssignAllPendingLinesToVersion;
using ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion;
using ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion.Models;
using ChangeTracker.Application.UseCases.Commands.DeleteAllPendingChangeLogLines;
using ChangeTracker.Application.UseCases.Commands.DeleteChangeLogLine;
using ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogLine;
using ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogs;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(5)]
    public class PendingChangeLogsController : ControllerBase
    {
        [HttpGet("products/{productId:Guid}/pending-changelogs")]
        [NeedsPermission(Permission.ViewPendingChangeLogLines)]
        public async Task<ActionResult<List<ChangeLogLineDto>>> GetPendingChangeLogsAsync(
            [FromServices] IGetPendingChangeLogs getPendingChangeLogs,
            Guid productId)
        {
            if (productId == Guid.Empty)
                return BadRequest(DefaultResponse.Create("ProductId cannot be empty."));

            var userId = HttpContext.GetUserId();
            var pendingChangeLogs = await getPendingChangeLogs.ExecuteAsync(userId, productId);

            return Ok(PendingChangeLogsDto.FromResponseModel(pendingChangeLogs));
        }

        [HttpGet("pending-changelogs/{changeLogLineId:Guid}")]
        [NeedsPermission(Permission.ViewPendingChangeLogLines)]
        public async Task<ActionResult<List<ChangeLogLineDto>>> GetPendingChangeLogLineAsync(
            [FromServices] IGetPendingChangeLogLine getPendingChangeLogLine,
            Guid changeLogLineId)
        {
            var userId = HttpContext.GetUserId();

            var presenter = new GetPendingChangeLogLineApiPresenter();
            await getPendingChangeLogLine.ExecuteAsync(presenter, userId, changeLogLineId);

            return presenter.Response;
        }

        [HttpPost("products/{productId:Guid}/pending-changelogs")]
        [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
        public async Task<ActionResult> AddPendingChangeLogLineAsync(
            [FromServices] IAddPendingChangeLogLine addPendingChangeLogLine,
            Guid productId,
            [FromBody] AddOrUpdateChangeLogLineDto pendingChangeLogLine)
        {
            var userId = HttpContext.GetUserId();
            var requestModel = new PendingChangeLogLineRequestModel(userId,
                productId,
                pendingChangeLogLine.Text,
                pendingChangeLogLine.Labels ?? new List<string>(0),
                pendingChangeLogLine.Issues ?? new List<string>(0));

            var presenter = new AddPendingChangeLogLineApiPresenter(HttpContext);
            await addPendingChangeLogLine.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpPost("pending-changelogs/{changeLogLineId:Guid}/move")]
        [NeedsPermission(Permission.MoveChangeLogLines)]
        public async Task<ActionResult> MovePendingChangeLogLineAsync(
            [FromServices] IAssignPendingLineToVersion assignPendingLineToVersion,
            Guid changeLogLineId,
            [FromBody] MoveChangeLogLineDto moveChangeLogLineDto)
        {
            if (moveChangeLogLineDto.TargetVersionId == Guid.Empty)
                return BadRequest(DefaultResponse.Create("TargetVersionId cannot be empty."));

            var requestModel =
                new VersionIdAssignmentRequestModel(moveChangeLogLineDto.TargetVersionId, changeLogLineId);

            var presenter = new AssignPendingLineToVersionToVersionApiPresenter();
            await assignPendingLineToVersion.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpPost("products/{productId:Guid}/pending-changelogs/move")]
        [NeedsPermission(Permission.MoveChangeLogLines)]
        public async Task<ActionResult> MoveAllPendingChangeLogLineAsync(
            [FromServices] IAssignAllPendingLinesToVersion assignAllPendingLinesToVersion,
            Guid productId,
            [FromBody] MoveChangeLogLineDto moveChangeLogLineDto)
        {
            if (moveChangeLogLineDto.TargetVersionId == Guid.Empty)
                return BadRequest(DefaultResponse.Create("TargetVersionId cannot be empty."));

            var requestModel =
                new Application.UseCases.Commands.AssignAllPendingLinesToVersion.Models.VersionIdAssignmentRequestModel(
                    productId, moveChangeLogLineDto.TargetVersionId);

            var presenter = new AssignAllPendingLinesApiPresenter();
            await assignAllPendingLinesToVersion.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpDelete("pending-changelogs/{changeLogLineId:Guid}")]
        [NeedsPermission(Permission.DeleteChangeLogLine)]
        public async Task<ActionResult> DeletePendingChangeLogLineAsync(
            [FromServices] IDeleteChangeLogLine deleteChangeLogLine,
            Guid changeLogLineId)
        {
            var requestModel = new DeleteChangeLogLineRequestModel(changeLogLineId, ChangeLogLineType.Pending);

            var presenter = new DeleteChangeLogLineApiPresenter();
            await deleteChangeLogLine.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpDelete("products/{productId:Guid}/pending-changelogs")]
        [NeedsPermission(Permission.DeleteChangeLogLine)]
        public async Task<ActionResult> DeleteAllPendingChangeLogLineAsync(
            [FromServices] IDeleteAllPendingChangeLogLines deleteAllPendingChangeLogLines,
            Guid productId)
        {
            if (productId == Guid.Empty)
                return BadRequest(DefaultResponse.Create("ProductId cannot be empty."));

            await deleteAllPendingChangeLogLines.ExecuteAsync(productId);

            return Ok(DefaultResponse.Create("Pending ChangeLogLines successfully deleted."));
        }
    }
}