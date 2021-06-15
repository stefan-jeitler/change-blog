using System;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.Commands.UpdateChangeLogLine
{
    public interface IUpdateChangeLogLineOutputPort : ILineParserOutput
    {
        void Updated(Guid changeLogLineId);
        void ChangeLogLineDoesNotExist();
        void Conflict(Conflict conflict);
        void LineWithSameTextAlreadyExists(string text);
        void RequestedLineIsNotPending(Guid changeLogLineId);
        void RequestedLineIsPending(Guid changeLogLineId);
    }
}