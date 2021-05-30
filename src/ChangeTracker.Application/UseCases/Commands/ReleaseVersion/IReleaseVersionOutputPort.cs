using System;

namespace ChangeTracker.Application.UseCases.Commands.ReleaseVersion
{
    public interface IReleaseVersionOutputPort
    {
        void VersionAlreadyReleased(Guid versionId);
        void VersionAlreadyDeleted(Guid versionId);
        void VersionReleased(Guid versionId);
        void Conflict(string reason);
        void VersionDoesNotExist(Guid versionId);
        void RelatedProductClosed(Guid productId);
    }
}