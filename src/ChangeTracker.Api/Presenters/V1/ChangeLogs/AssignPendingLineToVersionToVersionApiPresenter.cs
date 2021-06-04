using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.ChangeLogs
{
    public class AssignPendingLineToVersionToVersionApiPresenter : BaseApiPresenter, IAssignPendingLineToVersionOutputPort
    {
        public void InvalidVersionFormat(string version)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"Invalid version format '{version}'"));
        }

        public void VersionDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Version not found."));
        }

        public void MaxChangeLogLinesReached(int maxChangeLogLines)
        {
            Response = new ConflictObjectResult(
                DefaultResponse.Create($"The target version has reached the max lines count. Max lines: {maxChangeLogLines}"));
        }

        public void ChangeLogLineDoesNotExist(Guid changeLogLineId)
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("ChangeLogLine not found.", changeLogLineId));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }

        public void Assigned(Guid versionId, Guid changeLogLineId)
        {
            Response = new OkObjectResult(DefaultResponse.Create("Line successfully moved.", versionId));
        }

        public void ChangeLogLineIsNotPending(Guid changeLogLineId)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create("The given ChangeLogLine is not pending",
                changeLogLineId));
        }

        public void LineWithSameTextAlreadyExists(string text)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"The target version contains already lines with an identical text. Duplicate: {text}"));
        }
    }
}
