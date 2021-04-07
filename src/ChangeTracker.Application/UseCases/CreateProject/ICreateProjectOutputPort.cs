using System;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.Application.UseCases.CreateProject
{
    public interface ICreateProjectOutputPort
    {
        void AccountDoesNotExist();
        void AccountDeleted(Guid accountId);
        void InvalidName(string name);
        void ProjectAlreadyExists();
        void VersioningSchemeDoesNotExist();
        void Conflict(Conflict conflict);
        void Created(Guid accountId, Guid projectId);
    }
}