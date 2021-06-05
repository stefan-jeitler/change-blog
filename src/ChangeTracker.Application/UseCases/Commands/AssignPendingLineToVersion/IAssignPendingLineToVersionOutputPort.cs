using System;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion
{
    public interface IAssignPendingLineToVersionOutputPort
    {
        void InvalidVersionFormat(string version);
        void VersionDoesNotExist();
        void MaxChangeLogLinesReached(int maxChangeLogLines);
        void ChangeLogLineDoesNotExist(Guid changeLogLineId);
        void Conflict(Conflict conflict);
        void Assigned(Guid versionId, Guid changeLogLineId);
        void ChangeLogLineIsNotPending(Guid changeLogLineId);
        void LineWithSameTextAlreadyExists(string text);
    }
}