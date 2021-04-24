using System;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public interface IAddPendingLineOutputPort : ILineParserOutput
    {
        void ProjectDoesNotExist();
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
        void TooManyLines(int maxChangeLogLines);
        void LineWithSameTextAlreadyExists(string text);
    }
}