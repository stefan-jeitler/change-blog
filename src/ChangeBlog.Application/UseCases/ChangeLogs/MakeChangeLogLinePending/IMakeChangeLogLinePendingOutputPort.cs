using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.ChangeLogs.MakeChangeLogLinePending;

public interface IMakeChangeLogLinePendingOutputPort
{
    void WasMadePending(Guid changeLogLineId);
    void ChangeLogLineDoesNotExist();
    void ChangeLogLineIsAlreadyPending(Guid changeLogLineId);
    void VersionAlreadyReleased(Guid versionId);
    void VersionDeleted(Guid versionId);
    void TooManyPendingLines(int maxChangeLogLines);
    void Conflict(Conflict conflict);
    void LineWithSameTextAlreadyExists(Guid changeLogLineId, string duplicate);
}