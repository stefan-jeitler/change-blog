using System;
using ChangeTracker.Application.Services.ChangeLog;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public interface IAddChangeLogLineOutputPort : IChangeLogLineParsingOutput
    {
        void InvalidVersionFormat();
        void ProjectDoesNotExist();
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
        void VersionDoesNotExist(string version);
    }
}