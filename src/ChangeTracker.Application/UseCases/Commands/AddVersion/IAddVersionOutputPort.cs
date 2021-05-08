using System;

namespace ChangeTracker.Application.UseCases.Commands.AddVersion
{
    public interface IAddVersionOutputPort
    {
        void Created(Guid id);
        void InvalidVersionFormat(string version);
        void VersionDoesNotMatchScheme(string version);
        void ProjectDoesNotExist();
        void Conflict(string reason);
        void VersionAlreadyExists(string version);
        void ProjectClosed(Guid projectId);
    }
}