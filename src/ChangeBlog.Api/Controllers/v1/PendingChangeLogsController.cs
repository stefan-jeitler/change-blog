using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.DTOs.V1.ChangeLog;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Presenters.V1.ChangeLogs;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.ChangeLogs.AddPendingChangeLogLine;
using ChangeBlog.Application.UseCases.ChangeLogs.DeleteAllPendingChangeLogLines;
using ChangeBlog.Application.UseCases.ChangeLogs.DeleteChangeLogLine;
using ChangeBlog.Application.UseCases.ChangeLogs.GetPendingChangeLogLine;
using ChangeBlog.Application.UseCases.ChangeLogs.GetPendingChangeLogs;
using ChangeBlog.Application.UseCases.ChangeLogs.UpdateChangeLogLine;
using ChangeBlog.Application.UseCases.Versions.AssignAllPendingLinesToVersion;
using ChangeBlog.Application.UseCases.Versions.AssignPendingLineToVersion;
using ChangeBlog.Application.UseCases.Versions.AssignPendingLineToVersion.Models;
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
[SwaggerControllerOrder(3)]
public class PendingChangeLogsController : ControllerBase
{
    [HttpGet("products/{productId:Guid}/pending-changelogs", Name = "GetPendingChangeLogs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [NeedsPermission(Permission.ViewPendingChangeLogLines)]
    public async Task<ActionResult<List<ChangeLogLineDto>>> GetPendingChangeLogsAsync(
        [FromServices] IGetPendingChangeLogs getPendingChangeLogs,
        Guid productId)
    {
        var userId = HttpContext.GetUserId();
        var pendingChangeLogs = await getPendingChangeLogs.ExecuteAsync(userId, productId);

        return Ok(PendingChangeLogsDto.FromResponseModel(pendingChangeLogs));
    }

    [HttpGet("pending-changelogs/{changeLogLineId:Guid}", Name = "GetPendingChangeLogLine")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [NeedsPermission(Permission.ViewPendingChangeLogLines)]
    public async Task<ActionResult<PendingChangeLogLineDto>> GetPendingChangeLogLineAsync(
        [FromServices] IGetPendingChangeLogLine getPendingChangeLogLine,
        Guid changeLogLineId)
    {
        var userId = HttpContext.GetUserId();

        var presenter = new GetPendingChangeLogLineApiPresenter();
        await getPendingChangeLogLine.ExecuteAsync(presenter, userId, changeLogLineId);

        return presenter.Response;
    }

    [HttpPost("products/{productId:Guid}/pending-changelogs", Name = "AddPendingChangeLogLine")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
    public async Task<ActionResult<SuccessResponse>> AddPendingChangeLogLineAsync(
        [FromServices] IAddPendingChangeLogLine addPendingChangeLogLine,
        Guid productId,
        [FromBody] AddOrUpdateChangeLogLineDto pendingChangeLogLine)
    {
        var userId = HttpContext.GetUserId();
        var requestModel = new PendingChangeLogLineRequestModel(userId,
            productId,
            pendingChangeLogLine.Text,
            pendingChangeLogLine.Labels,
            pendingChangeLogLine.Issues);

        var presenter = new AddPendingChangeLogLineApiPresenter(HttpContext);
        await addPendingChangeLogLine.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }


    [HttpPatch("pending-changelogs/{changeLogLineId:Guid}", Name = "UpdatePendingChangeLogLine")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
    public async Task<ActionResult<SuccessResponse>> UpdateChangeLogLine(
        [FromServices] IUpdateChangeLogLine updateChangeLogLine,
        Guid changeLogLineId,
        [FromBody] PatchChangeLogLineDto patchChangeLogLineDto)
    {
        var requestModel = new UpdateChangeLogLineRequestModel(changeLogLineId,
            ChangeLogLineType.Pending,
            patchChangeLogLineDto.Text,
            patchChangeLogLineDto.Labels,
            patchChangeLogLineDto.Issues);

        var presenter = new UpdateChangeLogLineApiPresenter();
        await updateChangeLogLine.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpPost("pending-changelogs/{changeLogLineId:Guid}/move", Name = "MovePendingChangeLogLine")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.MoveChangeLogLines)]
    public async Task<ActionResult<SuccessResponse>> MovePendingChangeLogLineAsync(
        [FromServices] IAssignPendingLineToVersion assignPendingLineToVersion,
        Guid changeLogLineId,
        [FromBody] MoveChangeLogLineDto moveChangeLogLineDto)
    {
        if (moveChangeLogLineDto.TargetVersionId == Guid.Empty)
            return BadRequest(ErrorResponse.Create(ChangeBlogStrings.InvalidVersionId));

        var requestModel =
            new VersionIdAssignmentRequestModel(moveChangeLogLineDto.TargetVersionId, changeLogLineId);

        var presenter = new AssignPendingLineToVersionApiPresenter();
        await assignPendingLineToVersion.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpPost("products/{productId:Guid}/pending-changelogs/move", Name = "MoveAllPendingChangeLogs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.MoveChangeLogLines)]
    public async Task<ActionResult<SuccessResponse>> MoveAllPendingChangeLogLineAsync(
        [FromServices] IAssignAllPendingLinesToVersion assignAllPendingLinesToVersion,
        Guid productId,
        [FromBody] MoveChangeLogLineDto moveChangeLogLineDto)
    {
        if (moveChangeLogLineDto.TargetVersionId == Guid.Empty)
            return BadRequest(ErrorResponse.Create(ChangeBlogStrings.InvalidVersionId));

        var requestModel =
            new Application.UseCases.Versions.AssignAllPendingLinesToVersion.Models.VersionIdAssignmentRequestModel(
                productId, moveChangeLogLineDto.TargetVersionId);

        var presenter = new AssignAllPendingLinesApiPresenter();
        await assignAllPendingLinesToVersion.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpDelete("pending-changelogs/{changeLogLineId:Guid}", Name = "DeletePendingChangeLogLine")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [NeedsPermission(Permission.DeleteChangeLogLine)]
    public async Task<ActionResult<SuccessResponse>> DeletePendingChangeLogLineAsync(
        [FromServices] IDeleteChangeLogLine deleteChangeLogLine,
        Guid changeLogLineId)
    {
        var requestModel = new DeleteChangeLogLineRequestModel(changeLogLineId, ChangeLogLineType.Pending);

        var presenter = new DeleteChangeLogLineApiPresenter();
        await deleteChangeLogLine.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpDelete("products/{productId:Guid}/pending-changelogs", Name = "DeleteAllPendingChangeLogs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [NeedsPermission(Permission.DeleteChangeLogLine)]
    public async Task<ActionResult<SuccessResponse>> DeleteAllPendingChangeLogLineAsync(
        [FromServices] IDeleteAllPendingChangeLogLines deleteAllPendingChangeLogLines,
        Guid productId)
    {
        await deleteAllPendingChangeLogLines.ExecuteAsync(productId);

        return Ok(ErrorResponse.Create("Pending ChangeLogLines successfully deleted."));
    }
}