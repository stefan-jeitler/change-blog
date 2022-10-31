using System;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.DTOs.V1.ChangeLog;
using ChangeBlog.Api.Presenters.V1.ChangeLogs;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.ChangeLogs.AddChangeLogLine;
using ChangeBlog.Application.UseCases.ChangeLogs.AddChangeLogLine.Models;
using ChangeBlog.Application.UseCases.ChangeLogs.DeleteChangeLogLine;
using ChangeBlog.Application.UseCases.ChangeLogs.GetChangeLogLine;
using ChangeBlog.Application.UseCases.ChangeLogs.MakeAllChangeLogLinesPending;
using ChangeBlog.Application.UseCases.ChangeLogs.MakeChangeLogLinePending;
using ChangeBlog.Application.UseCases.ChangeLogs.UpdateChangeLogLine;
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
[SwaggerControllerOrder(6)]
public class ChangeLogsController : ControllerBase
{
    [HttpGet("changelogs/{changeLogLineId:Guid}", Name = "GetChangeLogLine")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
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

    [HttpDelete("changelogs/{changeLogLineId:Guid}", Name = "DeleteChangeLogLine")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [NeedsPermission(Permission.DeleteChangeLogLine)]
    public async Task<ActionResult<SuccessResponse>> DeleteChangeLogLineAsync(
        [FromServices] IDeleteChangeLogLine deleteChangeLogLine,
        Guid changeLogLineId)
    {
        var requestModel = new DeleteChangeLogLineRequestModel(changeLogLineId, ChangeLogLineType.NotPending);

        var presenter = new DeleteChangeLogLineApiPresenter();
        await deleteChangeLogLine.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpPost("versions/{versionId:Guid}/changelogs", Name = "AddChangeLogLine")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
    public async Task<ActionResult<SuccessResponse>> AddChangeLogLineAsync(
        [FromServices] IAddChangeLogLine addChangeLogLine,
        Guid versionId,
        [FromBody] AddOrUpdateChangeLogLineDto addChangeLogLineDto)
    {
        var userId = HttpContext.GetUserId();
        var requestModel = new VersionIdChangeLogLineRequestModel(userId,
            versionId,
            addChangeLogLineDto.Text,
            addChangeLogLineDto.Labels,
            addChangeLogLineDto.Issues);

        var presenter = new AddChangeLogLineApiPresenter(HttpContext);
        await addChangeLogLine.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpPatch("changelogs/{changeLogLineId:Guid}", Name = "UpdateChangeLogLine")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
    public async Task<ActionResult<SuccessResponse>> UpdateChangeLogLine(
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

    [HttpPost("changelogs/{changeLogLineId:Guid}/make-pending", Name = "MakeChangeLogLinePending")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.MoveChangeLogLines)]
    public async Task<ActionResult<SuccessResponse>> MakeChangeLogLinePendingAsync(
        [FromServices] IMakeChangeLogLinePending makeChangeLogLinePending,
        Guid changeLogLineId)
    {
        var presenter = new MakeChangeLogLinePendingApiPresenter();
        await makeChangeLogLinePending.ExecuteAsync(presenter, changeLogLineId);

        return presenter.Response;
    }

    [HttpPost("versions/{versionId:Guid}/changelogs/make-pending", Name = "MakeAllChangeLogLinesPending")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.MoveChangeLogLines)]
    public async Task<ActionResult<SuccessResponse>> MakeVersionChangeLogLinesPendingAsync(
        [FromServices] IMakeAllChangeLogLinesPending makeAllChangeLogLinesPending,
        Guid versionId)
    {
        var presenter = new MakeAllChangeLogLinesPendingApiPresenter();
        await makeAllChangeLogLinesPending.ExecuteAsync(presenter, versionId);

        return presenter.Response;
    }
}