using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs.V1.ChangeLog;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.Presenters.V1.ChangeLogs;
using ChangeTracker.Api.SwaggerUI;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.AddChangeLogLine;
using ChangeTracker.Application.UseCases.Commands.AddChangeLogLine.Models;
using ChangeTracker.Application.UseCases.Commands.DeleteChangeLogLine;
using ChangeTracker.Application.UseCases.Commands.MakeAllChangeLogLinesPending;
using ChangeTracker.Application.UseCases.Commands.MakeChangeLogLinePending;
using ChangeTracker.Application.UseCases.Queries.GetChangeLogLine;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(6)]
    public class ChangeLogsController : ControllerBase
    {
        [HttpGet("changelogs/{changeLogLineId:Guid}")]
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

        [HttpPost("changelogs/{changeLogLineId:Guid}/make-pending")]
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