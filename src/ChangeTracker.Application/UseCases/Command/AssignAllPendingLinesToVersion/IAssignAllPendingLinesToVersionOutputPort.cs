using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.Command.AssignAllPendingLinesToVersion
{
    public interface IAssignAllPendingLinesToVersionOutputPort
    {
        void Assigned(Guid versionId);
        void InvalidVersionFormat(string version);
        void VersionDoesNotExist();
        void TooManyLinesToAdd(uint remainingLinesToAdd);
        void Conflict(string reason);
        void NoPendingChangeLogLines();
        void LineWithSameTextAlreadyExists(List<string> texts);
    }
}
