using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Api.Presenters.V1.ChangeLogs;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Application.UseCases.Commands.Labels.AddChangeLogLineLabel;
using ChangeBlog.Application.UseCases.Commands.Labels.DeleteChangeLogLineLabel;
using ChangeBlog.Application.UseCases.Commands.Labels.SharedModels;
using ChangeBlog.Application.UseCases.Queries.GetLabels;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Controllers.V1;

[ApiController]
[Route("api/v1/changelogs/{changeLogLineId:Guid}/labels")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[SwaggerControllerOrder(7)]
public class LabelsController : ControllerBase
{
    [HttpGet(Name = "GetLabels")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [NeedsPermission(Permission.ViewChangeLogLines)]
    public async Task<ActionResult<List<string>>> GetChangeLogLineLabelsAsync([FromServices] IGetLabels getLabels,
        Guid changeLogLineId)
    {
        var labels = await getLabels.ExecuteAsync(changeLogLineId);

        return Ok(labels);
    }

    [HttpPut("{label}", Name = "AddLabel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
    public async Task<ActionResult<SuccessResponse>> AddLabelAsync(
        [FromServices] IAddChangeLogLineLabel addChangeLogLineLabel,
        Guid changeLogLineId, string label)
    {
        var requestModel = new ChangeLogLineLabelRequestModel(changeLogLineId, label);

        var presenter = new AddChangeLogLineLabelApiPresenter();
        await addChangeLogLineLabel.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }

    [HttpDelete("{label}", Name = "DeleteLabel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
    public async Task<ActionResult<SuccessResponse>> DeleteLabelAsync(
        [FromServices] IDeleteChangeLogLineLabel deleteChangeLogLineLabel,
        Guid changeLogLineId, string label)
    {
        var requestModel = new ChangeLogLineLabelRequestModel(changeLogLineId, label);

        var presenter = new DeleteChangeLogLineLabelApiPresenter();
        await deleteChangeLogLineLabel.ExecuteAsync(presenter, requestModel);

        return presenter.Response;
    }
}