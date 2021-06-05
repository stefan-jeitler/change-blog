using System;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.Application.UseCases.Commands.DeleteVersion
{
    public interface IDeleteVersionOutputPort
    {
        void VersionDoesNotExist(Guid versionId);
        void RelatedProductClosed(Guid productId);
        void VersionAlreadyDeleted(Guid versionId);
        void VersionAlreadyReleased(Guid versionId);
        void VersionDeleted(Guid versionId);
        void Conflict(Conflict conflict);
    }
}