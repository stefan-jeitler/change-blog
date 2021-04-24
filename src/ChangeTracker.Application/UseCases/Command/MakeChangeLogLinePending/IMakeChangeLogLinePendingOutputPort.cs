using System;

namespace ChangeTracker.Application.UseCases.Command.MakeChangeLogLinePending
{
    public interface IMakeChangeLogLinePendingOutputPort
    {
        void WasMadePending(Guid lineId);
        void ChangeLogLineDoesNotExist();
        void ChangeLogLineIsAlreadyPending();
        void VersionAlreadyReleased();
        void VersionDeleted();
        void TooManyPendingLines(int maxChangeLogLines);
        void Conflict(string reason);
        void LineWithSameTextAlreadyExists(string text);
    }
}