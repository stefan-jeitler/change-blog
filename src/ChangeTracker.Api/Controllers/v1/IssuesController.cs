using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.Presenters.V1.ChangeLogs;
using ChangeTracker.Api.SwaggerUI;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.Issues.AddChangeLogLineIssue;
using ChangeTracker.Application.UseCases.Commands.Issues.DeleteChangeLogLineIssue;
using ChangeTracker.Application.UseCases.Commands.Issues.SharedModels;
using ChangeTracker.Application.UseCases.Queries.GetIssues;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/changelogs/{changeLogLineId:Guid}/issues")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(8)]
    public class IssuesController : ControllerBase
    {
        [HttpGet]
        [NeedsPermission(Permission.ViewChangeLogLines)]
        public async Task<ActionResult<List<string>>> GetChangeLogLineIssuesAsync([FromServices] IGetIssues getIssues,
            Guid changeLogLineId)
        {
            var issues = await getIssues.ExecuteAsync(changeLogLineId);

            return Ok(issues);
        }

        [HttpPatch("{issue}")]
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