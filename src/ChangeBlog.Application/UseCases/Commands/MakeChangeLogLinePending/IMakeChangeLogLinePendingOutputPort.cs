using System;
using ChangeBlog.Application.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.MakeChangeLogLinePending;

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