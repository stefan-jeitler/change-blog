using System;
using ChangeBlog.Application.ChangeLogLineParser;
using ChangeBlog.Application.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.AddPendingChangeLogLine
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
