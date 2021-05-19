using System;

namespace ChangeTracker.Application.UseCases.Commands.ReleaseVersion
{
    public interface IReleaseVersionOutputPort
    {
        void VersionAlreadyReleased();
        void VersionDeleted();
        void VersionReleased(Guid versionId);
        void Conflict(string reason);
        void VersionDoesNotExist();
        void RelatedProductClosed(Guid productId);
    }
}