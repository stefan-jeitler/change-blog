using System;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.ChangeLogLineParser;

namespace ChangeBlog.Application.UseCases.ChangeLogs.UpdateChangeLogLine;

public interface IUpdateChangeLogLineOutputPort : ILineParserOutput
{
    void Updated(Guid changeLogLineId);
    void ChangeLogLineDoesNotExist();
    void Conflict(Conflict conflict);
    void LineWithSameTextAlreadyExists(string text);
    void RequestedLineIsNotPending(Guid changeLogLineId);
    void RequestedLineIsPending(Guid changeLogLineId);
}