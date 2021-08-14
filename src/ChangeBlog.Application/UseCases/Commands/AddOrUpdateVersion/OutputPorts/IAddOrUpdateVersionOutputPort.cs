using System;
using ChangeBlog.Application.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.OutputPorts
{
    public interface IAddOrUpdateVersionOutputPort : IAddVersionOutputPort
    {
        void VersionAlreadyDeleted(Guid versionId);
        void VersionAlreadyReleased(Guid versionId);
        new void InvalidVersionFormat(string version);
        new void InvalidVersionName(string name);
        void VersionUpdated(Guid versionId);
        new void RelatedProductClosed(Guid versionId);
        void UpdateConflict(Conflict conflict);
    }
}
