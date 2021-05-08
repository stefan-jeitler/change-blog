using System;

namespace ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion
{
    public interface IAssignPendingLineOutputPort
    {
        void InvalidVersionFormat(string version);
        void VersionDoesNotExist();
        void MaxChangeLogLinesReached(int maxChangeLogLines);
        void ChangeLogLineDoesNotExist();
        void Conflict(string reason);
        void Assigned(Guid versionId, Guid changeLogLineId);
        void ChangeLogLineIsNotPending(Guid changeLogLineId);
        void LineWithSameTextAlreadyExists(string text);
    }
}