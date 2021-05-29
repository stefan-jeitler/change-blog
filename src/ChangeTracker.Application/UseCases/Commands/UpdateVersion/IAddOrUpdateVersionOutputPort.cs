using System;
using ChangeTracker.Application.UseCases.Commands.AddVersion;

namespace ChangeTracker.Application.UseCases.Commands.UpdateVersion
{
    public interface IAddOrUpdateVersionOutputPort : IAddVersionOutputPort
    {
        void VersionDoesNotExist();
        void VersionAlreadyDeleted();
        void VersionAlreadyReleased();
        new void InvalidVersionFormat(string version);
        new void InvalidVersionName(string name);
        void VersionWithSameValueAlreadyExists(string value);
        void VersionUpdated(Guid versionId);
        new void Conflict(string reason);
        void RelatedProductClosed(Guid productId);
        new void VersionDoesNotMatchScheme(string version);
    }
}