using System;
using ChangeTracker.Application.UseCases.Queries.SharedModels;

namespace ChangeTracker.Application.UseCases.Queries.GetChangeLogLine
{
    public interface IGetChangeLogLineOutputPort
    {
        void LineDoesNotExists(Guid changeLogLineId);
        void LineIsPending(Guid changeLogLineId);
        void LineFound(ChangeLogLineResponseModel responseModel);
    }
}