using System;
using ChangeBlog.Application.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.AssignPendingLineToVersion
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
        void TargetVersionBelongsToDifferentProduct(Guid changeLogLineProductId, Guid targetVersionProductId);
    }
}
