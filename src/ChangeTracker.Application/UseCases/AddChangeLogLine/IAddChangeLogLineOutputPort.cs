using System;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public interface IAddChangeLogLineOutputPort : IChangeLogLineParsingOutput
    {
        void InvalidVersionFormat();
        void ProjectDoesNotExist();
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
        void VersionDoesNotExist(string version);
        void VersionAlreadyReleased(string version);
        void VersionDeleted(string version);
    }
}