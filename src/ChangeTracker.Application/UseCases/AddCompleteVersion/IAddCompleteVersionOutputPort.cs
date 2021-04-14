using System;
using ChangeTracker.Application.Services.ChangeLog;

namespace ChangeTracker.Application.UseCases.AddCompleteVersion
{
    public interface IAddCompleteVersionOutputPort : IChangeLogLineParsingOutput
    {
        void ProjectDoesNotExist();
        void InvalidVersionFormat(string version);
        void VersionDoesNotMatchScheme(string version);
        void Created(Guid versionId);
        void Conflict(string reason);
        void VersionAlreadyExists(string version);
    }
}