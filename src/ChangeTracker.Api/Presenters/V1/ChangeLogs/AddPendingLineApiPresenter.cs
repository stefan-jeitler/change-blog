using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.UseCases.Commands.AddPendingChangeLogLine;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.ChangeLogs
{
    public class AddPendingLineApiPresenter : BaseApiPresenter, IAddPendingLineOutputPort
    {
        private readonly HttpContext _httpContext;

        public AddPendingLineApiPresenter(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public void ProductDoesNotExist(Guid productId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(productId)] = productId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("Product does not exist.", resourceIds));
        }

        public void Created(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(changeLogLineId)] = changeLogLineId.ToString()
            };

            var location = _httpContext.CreateLinkTo($"api/v1/pending-changelogs/{changeLogLineId}");
            Response = new CreatedResult(location, DefaultResponse.Create("Pending ChangeLogLine successfully added.", resourceIds));
        }

        public void Conflict(Conflict conflict)
        {
            Response = conflict.ToResponse();
        }

        public void TooManyLines(int maxChangeLogLines)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"Too many lines. Max lines: {maxChangeLogLines}"));
        }

        public void LinesWithSameTextsAreNotAllowed(string duplicate)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"Lines with the same text are not allowed. Duplicate: '{duplicate}'"));
        }

        public void InvalidChangeLogLineText(string text)
        {
            Response = new BadRequestObjectResult(DefaultResponse.Create($"Invalid change log text '{text}'."));
        }

        public void InvalidIssue(string changeLogText, string issue)
        {
            Response = new BadRequestObjectResult(
                DefaultResponse.Create($"Invalid issue '{issue}' for change log '{changeLogText}'."));
        }

        public void TooManyIssues(string changeLogText, int maxIssues)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create(
                    $"The change log '{changeLogText}' has too many issues. Max issues: '{maxIssues}'."));
        }

        public void InvalidLabel(string changeLogText, string label)
        {
            Response = new BadRequestObjectResult(
                DefaultResponse.Create($"Invalid label '{label}' for change log '{changeLogText}'."));
        }

        public void TooManyLabels(string changeLogText, int maxLabels)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create(
                    $"The change log '{changeLogText}' has too many labels. Max labels: '{maxLabels}'."));
        }
    }
}
