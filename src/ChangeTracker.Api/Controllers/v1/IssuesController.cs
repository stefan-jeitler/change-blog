using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Presenters.V1.ChangeLogs;
using ChangeTracker.Api.SwaggerUI;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.Issues.AddChangeLogLineIssue;
using ChangeTracker.Application.UseCases.Commands.Issues.DeleteChangeLogLineIssue;
using ChangeTracker.Application.UseCases.Commands.Issues.SharedModels;
using ChangeTracker.Application.UseCases.Queries.GetIssues;
using ChangeTracker.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/changelogs/{changeLogLineId:Guid}/issues")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status403Forbidden)]
    [SwaggerControllerOrder(8)]
    public class IssuesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [NeedsPermission(Permission.ViewChangeLogLines)]
        public async Task<ActionResult<List<string>>> GetChangeLogLineIssuesAsync([FromServices] IGetIssues getIssues,
            Guid changeLogLineId)
        {
            var issues = await getIssues.ExecuteAsync(changeLogLineId);

            return Ok(issues);
        }

        [HttpPatch("{issue}")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status422UnprocessableEntity)]
        [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
        public async Task<ActionResult> AddIssueAsync(
            [FromServices] IAddChangeLogLineIssue addChangeLogLineIssue,
            Guid changeLogLineId, string issue)
        {
            var requestModel = new ChangeLogLineIssueRequestModel(changeLogLineId, issue);

            var presenter = new AddChangeLogLineIssueApiPresenter();
            await addChangeLogLineIssue.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpDelete("{issue}")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status409Conflict)]
        [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
        public async Task<ActionResult> DeleteIssueAsync(
            [FromServices] IDeleteChangeLogLineIssue deleteChangeLogLineIssue,
            Guid changeLogLineId, string issue)
        {
            var requestModel = new ChangeLogLineIssueRequestModel(changeLogLineId, issue);

            var presenter = new DeleteChangeLogLineIssueApiPresenter();
            await deleteChangeLogLineIssue.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }
    }
}