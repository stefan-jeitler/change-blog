using System;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.Commands.AddChangeLogLine
{
    public interface IAddLineOutputPort : ILineParserOutput
    {
        void InvalidVersionFormat();
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
        void VersionDoesNotExist();
        void TooManyLines(int maxChangeLogLines);
        void LineWithSameTextAlreadyExists(string text);
    }
}