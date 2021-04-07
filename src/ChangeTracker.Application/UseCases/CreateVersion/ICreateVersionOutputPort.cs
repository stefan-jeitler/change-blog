using System;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.Application.UseCases.CreateVersion
{
    public interface ICreateVersionOutputPort
    {
        void Created(Guid id);
        void InvalidVersionFormat(string version);
        void VersionDoesNotMatchScheme(string version);
        void ProjectDoesNotExist();
        void Conflict(Conflict conflict);
    }
}