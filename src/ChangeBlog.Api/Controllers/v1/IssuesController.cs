using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Authorization;
using ChangeBlog.Api.Presenters.V1.ChangeLogs;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Swagger;
using ChangeBlog.Application.UseCases.Commands.Issues.AddChangeLogLineIssue;
using ChangeBlog.Application.UseCases.Commands.Issues.DeleteChangeLogLineIssue;
using ChangeBlog.Application.UseCases.Commands.Issues.SharedModels;
using ChangeBlog.Application.UseCases.Queries.GetIssues;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Controllers.V1;

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