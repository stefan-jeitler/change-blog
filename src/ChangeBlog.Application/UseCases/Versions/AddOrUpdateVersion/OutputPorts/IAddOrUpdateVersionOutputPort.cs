using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion.OutputPorts;

public interface IAddOrUpdateVersionOutputPort : IAddVersionOutputPort
{
    void VersionAlreadyDeleted(Guid versionId);
    void VersionAlreadyReleased(Guid versionId);
    new void InvalidVersionFormat(string version);
    new void InvalidVersionName(string name);
    void VersionUpdated(Guid versionId);
    new void RelatedProductFreezed(Guid versionId);
    void UpdateConflict(Conflict conflict);
}