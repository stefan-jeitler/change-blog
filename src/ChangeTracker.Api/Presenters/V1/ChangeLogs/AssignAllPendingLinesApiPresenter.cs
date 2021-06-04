using System;
using System.Collections.Generic;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.AssignAllPendingLinesToVersion;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.ChangeLogs
{
    public class AssignAllPendingLinesApiPresenter : BaseApiPresenter, IAssignAllPendingLinesToVersionOutputPort
    {
        public void Assigned(Guid versionId)
        {
            Response = new OkObjectResult(DefaultResponse.Create("Lines successfully moved.", versionId));
        }

        public void InvalidVersionFormat(string version)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"Invalid version format '{version}'"));
        }

        public void VersionDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Version not found."));
        }

        public void TooManyLinesToAdd(uint remainingLinesToAdd)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"Too many lines. Remaining lines: {remainingLinesToAdd}"));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }

        public void NoPendingChangeLogLines()
        {
            Response = new OkObjectResult(DefaultResponse.Create("There are no Lines to assign."));
        }

        public void LineWithSameTextAlreadyExists(List<string> texts)
        {
            Response = new UnprocessableEntityObjectResult(
                DefaultResponse.Create($"The target version contains already lines with an identical text. Duplicates: {string.Join(", ", texts)}"));
        }
    }
}