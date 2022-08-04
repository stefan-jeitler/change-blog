using System;
using ChangeBlog.Application.UseCases.SharedModels;

namespace ChangeBlog.Application.UseCases.ChangeLogs.GetChangeLogLine;

public interface IGetChangeLogLineOutputPort
{
    void LineDoesNotExists(Guid changeLogLineId);
    void LineIsPending(Guid changeLogLineId);
    void LineFound(ChangeLogLineResponseModel responseModel);
}