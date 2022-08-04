using System;

namespace ChangeBlog.Application.UseCases.ChangeLogs.GetPendingChangeLogLine;

public interface IGetPendingChangeLogLineOutputPort
{
    void LineDoesNotExist(Guid changeLogLineId);
    void LineIsNotPending(Guid changeLogLineId);
    void LineFound(PendingChangeLogLineResponseModel responseModel);
}