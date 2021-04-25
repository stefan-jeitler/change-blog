using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.Command.MakeAllChangeLogLinesPending
{
    public interface IMakeAllChangeLogLinesPendingOutputPort
    {
        void VersionDoesNotExist();
        void VersionAlreadyReleased();
        void VersionClosed();
        void TooManyPendingLines(int maxChangeLogLines);
        void LineWithSameTextAlreadyExists(List<string> text);
        void Conflict(string reason);
        void MadePending(int count);
        void InvalidVersionFormat(string version);
    }
}