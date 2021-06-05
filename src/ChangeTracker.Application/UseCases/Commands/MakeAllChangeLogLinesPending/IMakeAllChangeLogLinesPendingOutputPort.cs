using System;
using System.Collections.Generic;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.Application.UseCases.Commands.MakeAllChangeLogLinesPending
{
    public interface IMakeAllChangeLogLinesPendingOutputPort
    {
        void VersionDoesNotExist();
        void VersionAlreadyReleased();
        void VersionClosed();
        void TooManyPendingLines(int maxChangeLogLines);
        void LineWithSameTextAlreadyExists(List<string> text);
        void Conflict(Conflict conflict);
        void MadePending(int count);
        void InvalidVersionFormat(string version);
    }
}