using System;
using ChangeTracker.Application.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.AddCompleteVersion
{
    public interface IAddCompleteVersionOutputPort : ILineParserOutput
    {
        void ProjectDoesNotExist();
        void InvalidVersionFormat(string version);
        void VersionDoesNotMatchScheme(string version);
        void Created(Guid versionId);
        void Conflict(string reason);
        void VersionAlreadyExists(string version);
        void TooManyLines(int maxChangeLogLines);
    }
}