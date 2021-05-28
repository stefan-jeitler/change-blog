using System;

namespace ChangeTracker.Application.UseCases.Commands.DeleteVersion
{
    public interface IDeleteVersionOutputPort
    {
        void VersionDoesNotExist();
        void RelatedProductClosed(Guid productId);
        void VersionAlreadyDeleted();
        void VersionAlreadyReleased();
        void VersionDeleted(Guid versionId);
        void Conflict(string reason);
    }
}