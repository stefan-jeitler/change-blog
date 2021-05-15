using System;

namespace ChangeTracker.Application.UseCases.Commands.AddProject
{
    public interface IAddProjectOutputPort
    {
        void AccountDoesNotExist();
        void AccountDeleted(Guid accountId);
        void InvalidName(string name);
        void ProjectAlreadyExists(Guid projectId);
        void VersioningSchemeDoesNotExist();
        void Conflict(string reason);
        void Created(Guid accountId, Guid projectId);
    }
}