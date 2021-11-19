using System;
using ChangeBlog.Application.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.DeleteChangeLogLine;

public interface IDeleteChangeLogLineOutputPort
{
    public void LineDoesNotExist(Guid changeLogLineId);
    void LineDeleted(Guid changeLogLineId);
    void Conflict(Conflict conflict);
    void RequestedLineIsNotPending(Guid changeLogLineId);
    void RequestedLineIsPending(Guid changeLogLineId);
}