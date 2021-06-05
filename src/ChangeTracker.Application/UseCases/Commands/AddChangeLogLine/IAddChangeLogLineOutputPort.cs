using System;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.Commands.AddChangeLogLine
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