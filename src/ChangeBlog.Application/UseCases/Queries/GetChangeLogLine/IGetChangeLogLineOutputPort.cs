using System;
using ChangeBlog.Application.UseCases.Queries.SharedModels;

namespace ChangeBlog.Application.UseCases.Queries.GetChangeLogLine
{
    public interface IGetChangeLogLineOutputPort
    {
        void LineDoesNotExists(Guid changeLogLineId);
        void LineIsPending(Guid changeLogLineId);
        void LineFound(ChangeLogLineResponseModel responseModel);
    }
}
