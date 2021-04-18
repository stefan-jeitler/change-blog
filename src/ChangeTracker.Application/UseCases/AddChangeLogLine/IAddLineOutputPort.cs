using System;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public interface IAddLineOutputPort : ILineParserOutput
    {
        void InvalidVersionFormat();
        void ProjectDoesNotExist();
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
        void VersionDoesNotExist(string version);
        void RelatedVersionAlreadyReleased(string version);
        void RelatedVersionDeleted(string version);
        void TooManyLines(int maxChangeLogLines);
    }
}