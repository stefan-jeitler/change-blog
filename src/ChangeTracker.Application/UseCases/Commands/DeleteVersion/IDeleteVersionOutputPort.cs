using System;

namespace ChangeTracker.Application.UseCases.Commands.DeleteVersion
{
    public interface IDeleteVersionOutputPort
    {
        void VersionDoesNotExist();
        void ProductClosed(Guid productId);
        void VersionAlreadyDeleted();
        void VersionAlreadyReleased();
        void VersionDeleted(Guid versionId);
        void Conflict(string reason);
    }
}