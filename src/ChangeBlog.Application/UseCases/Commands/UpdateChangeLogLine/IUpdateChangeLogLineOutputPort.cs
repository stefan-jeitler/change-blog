using System;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.Services.ChangeLogLineParsing;

namespace ChangeBlog.Application.UseCases.Commands.UpdateChangeLogLine
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
