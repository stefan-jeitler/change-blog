using System;

namespace ChangeTracker.Application.Services.NotReleasedVersion
{
    public interface INotReleasedVersionOutputPort
    {
        void ProjectDoesNotExist();
        void VersionDoesNotExist();
        void VersionAlreadyReleased(DateTime releaseAt);
        void VersionDeleted(DateTime deletedAt);
    }
}