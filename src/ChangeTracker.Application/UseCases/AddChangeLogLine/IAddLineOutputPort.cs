using System;
using ChangeTracker.Application.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public interface IAddLineOutputPort : ILineParserOutput
    {
        void InvalidVersionFormat();
        void ProjectDoesNotExist();
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
        void VersionDoesNotExist(string version);
        void VersionAlreadyReleased(string version);
        void VersionDeleted(string version);
        void TooManyLines(int maxChangeLogLines);
    }
}