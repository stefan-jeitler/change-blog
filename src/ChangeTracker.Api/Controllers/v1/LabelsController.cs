using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.Presenters.V1.ChangeLogs;
using ChangeTracker.Api.SwaggerUI;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.Labels.AddChangeLogLineLabel;
using ChangeTracker.Application.UseCases.Commands.Labels.DeleteChangeLogLineLabel;
using ChangeTracker.Application.UseCases.Commands.Labels.SharedModels;
using ChangeTracker.Application.UseCases.Queries.GetLabels;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/changelogs/{changeLogLineId:Guid}/labels")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(7)]
    public class LabelsController : ControllerBase
    {
        [HttpGet]
        [NeedsPermission(Permission.ViewChangeLogLines)]
        public async Task<ActionResult<List<string>>> GetChangeLogLineLabelsAsync([FromServices] IGetLabels getLabels,
            Guid changeLogLineId)
        {
            var labels = await getLabels.ExecuteAsync(changeLogLineId);

            return Ok(labels);
        }

        [HttpPost]
        [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
        public async Task<ActionResult> AddLabelAsync(
            [FromServices] IAddChangeLogLineLabel addChangeLogLineLabel,
            Guid changeLogLineId,
            [FromBody]
            [Required]
            string label)
        {
            var requestModel = new ChangeLogLineLabelRequestModel(changeLogLineId, label);

            var presenter = new AddChangeLogLineLabelApiPresenter();
            await addChangeLogLineLabel.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }

        [HttpDelete]
        [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
        public async Task<ActionResult> DeleteLabelAsync(
            [FromServices] IDeleteChangeLogLineLabel deleteChangeLogLineLabel, 
            Guid changeLogLineId,
            [FromBody] 
            [Required]
            string label)
        {
            var requestModel = new ChangeLogLineLabelRequestModel(changeLogLineId, label);

            var presenter = new DeleteChangeLogLineLabelApiPresenter();
            await deleteChangeLogLineLabel.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }
    }
}