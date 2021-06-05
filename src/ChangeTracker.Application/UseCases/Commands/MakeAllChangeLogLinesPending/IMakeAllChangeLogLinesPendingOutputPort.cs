using System;
using System.Collections.Generic;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.Application.UseCases.Commands.MakeAllChangeLogLinesPending
{
    public interface IMakeAllChangeLogLinesPendingOutputPort
    {
        void VersionDoesNotExist();
        void VersionAlreadyReleased(Guid versionId);
        void VersionDeleted(Guid versionId);
        void TooManyPendingLines(int maxChangeLogLines);
        void LineWithSameTextAlreadyExists(List<string> text);
        void Conflict(Conflict conflict);
        void MadePending(Guid productId, int count);
        void InvalidVersionFormat(string version);
    }
}