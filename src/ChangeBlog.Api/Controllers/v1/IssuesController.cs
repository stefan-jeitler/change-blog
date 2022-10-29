using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Presenters.V1.ChangeLogs;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.ChangeLogs.GetIssues;
using ChangeBlog.Application.UseCases.ChangeLogs.Issues.AddChangeLogLineIssue;
using ChangeBlog.Application.UseCases.ChangeLogs.Issues.DeleteChangeLogLineIssue;
using ChangeBlog.Application.UseCases.ChangeLogs.Issues.SharedModels;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Controllers.v1;

[ApiController]
[Route("api/v1/changelogs/{changeLogLineId:Guid}/issues")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[SwaggerControllerOrder(8)]
public class IssuesController : ControllerBase
{
    [HttpGet(Name = "GetIssues")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [NeedsPermission(Permission.ViewChangeLogLines)]
    public async Task<ActionResult<List<string>>> GetChangeLogLineIssuesAsync([FromServices] IGetIssues getIssues,
        Guid changeLogLineId)
    {
        var issues = await getIssues.ExecuteAsync(changeLogLineId);

        return Ok(issues);
    }

    [HttpPut("{issue}", Name = "AddIssue")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
    public async Task<ActionResult<SuccessResponse>> AddIssueAsync(
        [FromServices] IAddChangeLogLineIssue addChangeLogLineIssue,
        Guid changeLogLineId, string issue)
    {
        var requestModel = new ChangeLogLineIssueRequestModel(changeLogLineId, issue);

        var presenter = new AddChangeLogLineIssueApiPresenter();
        await addChangeLogLineIssue.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpDelete("{issue}", Name = "DeleteIssue")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
    public async Task<ActionResult<SuccessResponse>> DeleteIssueAsync(
        [FromServices] IDeleteChangeLogLineIssue deleteChangeLogLineIssue,
        Guid changeLogLineId, string issue)
    {
        var requestModel = new ChangeLogLineIssueRequestModel(changeLogLineId, issue);

        var presenter = new DeleteChangeLogLineIssueApiPresenter();
        await deleteChangeLogLineIssue.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }
}