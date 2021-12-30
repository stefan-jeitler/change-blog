using System;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.ChangeLogLineParser;

namespace ChangeBlog.Application.UseCases.Commands.AddPendingChangeLogLine;

public interface IAddPendingChangeLogLineOutputPort : ILineParserOutput
{
    void ProductDoesNotExist(Guid productId);
    void Created(Guid changeLogLineId);
    void Conflict(Conflict conflict);
    void TooManyLines(int maxChangeLogLines);
    void LinesWithSameTextsAreNotAllowed(Guid changeLogLineId, string text);
}