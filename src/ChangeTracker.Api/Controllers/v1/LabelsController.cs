using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Presenters.V1.ChangeLogs;
using ChangeTracker.Api.SwaggerUI;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.Labels.AddChangeLogLineLabel;
using ChangeTracker.Application.UseCases.Commands.Labels.DeleteChangeLogLineLabel;
using ChangeTracker.Application.UseCases.Commands.Labels.SharedModels;
using ChangeTracker.Application.UseCases.Queries.GetLabels;
using ChangeTracker.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/changelogs/{changeLogLineId:Guid}/labels")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status403Forbidden)]
    [SwaggerControllerOrder(7)]
    public class LabelsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [NeedsPermission(Permission.ViewChangeLogLines)]
        public async Task<ActionResult<List<string>>> GetChangeLogLineLabelsAsync([FromServices] IGetLabels getLabels,
            Guid changeLogLineId)
        {
            var labels = await getLabels.ExecuteAsync(changeLogLineId);

            return Ok(labels);
        }

        [HttpPatch("{label}")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status422UnprocessableEntity)]
        [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
        public async Task<ActionResult> AddLabelAsync(
            [FromServices] IAddChangeLogLineLabel addChangeLogLineLabel,
            Guid changeLogLineId, string label)
        {
            var requestModel = new ChangeLogLineLabelRequestModel(changeLogLineId, label);

            var presenter = new AddChangeLogLineLabelApiPresenter();
            await addChangeLogLineLabel.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpDelete("{label}")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status409Conflict)]
        [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
        public async Task<ActionResult> DeleteLabelAsync(
            [FromServices] IDeleteChangeLogLineLabel deleteChangeLogLineLabel,
            Guid changeLogLineId, string label)
        {
            var requestModel = new ChangeLogLineLabelRequestModel(changeLogLineId, label);

            var presenter = new DeleteChangeLogLineLabelApiPresenter();
            await deleteChangeLogLineLabel.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }
    }
}