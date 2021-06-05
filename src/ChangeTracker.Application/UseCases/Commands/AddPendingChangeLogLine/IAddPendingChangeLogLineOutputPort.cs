using System;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.Commands.AddPendingChangeLogLine
{
    public interface IAddPendingChangeLogLineOutputPort : ILineParserOutput
    {
        void ProductDoesNotExist(Guid productId);
        void Created(Guid changeLogLineId);
        void Conflict(Conflict conflict);
        void TooManyLines(int maxChangeLogLines);
        void LinesWithSameTextsAreNotAllowed(Guid changeLogLineId, string text);
    }
}