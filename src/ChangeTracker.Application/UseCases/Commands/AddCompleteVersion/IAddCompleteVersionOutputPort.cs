using System;
using System.Collections.Generic;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.Commands.AddCompleteVersion
{
    public interface IAddCompleteVersionOutputPort : ILineParserOutput
    {
        void ProjectDoesNotExist();
        void ProjectClosed();
        void InvalidVersionFormat(string version);
        void VersionDoesNotMatchScheme(string version);
        void Created(Guid versionId);
        void Conflict(string reason);
        void VersionAlreadyExists(string version);
        void TooManyLines(int maxChangeLogLines);
        void LinesWithSameTextsAreNotAllowed(IList<string> duplicates);
    }
}