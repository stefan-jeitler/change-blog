using System;
using ChangeTracker.Application.UseCases.Commands.AddVersion;

namespace ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion
{
    public interface IAddOrUpdateVersionOutputPort : IAddVersionOutputPort
    {
        void VersionAlreadyDeleted(Guid versionId);
        void VersionAlreadyReleased(Guid versionId);
        new void InvalidVersionFormat(string version);
        new void InvalidVersionName(string name);
        void VersionUpdated(Guid versionId);
        new void Conflict(string reason);
        void RelatedProductClosed(Guid versionId);
    }
}