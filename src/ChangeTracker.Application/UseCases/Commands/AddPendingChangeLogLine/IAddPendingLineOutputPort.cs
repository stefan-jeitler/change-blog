using System;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.Commands.AddPendingChangeLogLine
{
    public interface IAddPendingLineOutputPort : ILineParserOutput
    {
        void ProductDoesNotExist(Guid productId);
        void Created(Guid changeLogLineId);
        void Conflict(Conflict conflict);
        void TooManyLines(int maxChangeLogLines);
        void LinesWithSameTextsAreNotAllowed(string text);
    }
}