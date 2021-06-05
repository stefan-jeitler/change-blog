using System;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.Application.UseCases.Commands.MakeChangeLogLinePending
{
    public interface IMakeChangeLogLinePendingOutputPort
    {
        void WasMadePending(Guid changeLogLineId);
        void ChangeLogLineDoesNotExist();
        void ChangeLogLineIsAlreadyPending(Guid changeLogLineId);
        void VersionAlreadyReleased(Guid versionId);
        void VersionClosed(Guid versionId);
        void TooManyPendingLines(int maxChangeLogLines);
        void Conflict(Conflict conflict);
        void LineWithSameTextAlreadyExists(string text);
    }
}