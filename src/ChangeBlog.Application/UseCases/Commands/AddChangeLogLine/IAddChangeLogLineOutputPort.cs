using System;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.Services.ChangeLogLineParsing;

namespace ChangeBlog.Application.UseCases.Commands.AddChangeLogLine
{
    public interface IAddChangeLogLineOutputPort : ILineParserOutput
    {
        void InvalidVersionFormat(string version);
        void Created(Guid changeLogLineId);
        void Conflict(Conflict conflict);
        void VersionDoesNotExist();
        void TooManyLines(int maxChangeLogLines);
        void LineWithSameTextAlreadyExists(Guid changeLogLineId, string duplicate);
    }
}
