using System;
using System.Collections.Generic;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.ChangeLogs.MakeAllChangeLogLinesPending;

public interface IMakeAllChangeLogLinesPendingOutputPort
{
    void VersionDoesNotExist();
    void VersionAlreadyReleased(Guid versionId);
    void VersionDeleted(Guid versionId);
    void TooManyPendingLines(int maxChangeLogLines);
    void LineWithSameTextAlreadyExists(IEnumerable<string> text);
    void Conflict(Conflict conflict);
    void MadePending(Guid productId, int count);
    void InvalidVersionFormat(string version);
}