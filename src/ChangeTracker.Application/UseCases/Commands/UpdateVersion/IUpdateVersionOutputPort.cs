using System;

namespace ChangeTracker.Application.UseCases.Commands.UpdateVersion
{
    public interface IUpdateVersionOutputPort
    {
        void VersionDoesNotExist();
        void VersionAlreadyDeleted();
        void VersionAlreadyReleased();
        void InvalidVersionFormat(string version);
        void InvalidVersionName(string name);
        void VersionWithSameValueAlreadyExists(string value);
        void VersionUpdated(Guid versionId);
        void Conflict(string reason);
        void RelatedProductClosed(Guid productId);
        void VersionDoesNotMatchScheme(string version);
    }
}