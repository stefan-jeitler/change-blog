using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Versions.DeleteVersion;

public interface IDeleteVersionOutputPort
{
    void VersionDoesNotExist(Guid versionId);
    void RelatedProductFreezed(Guid productId);
    void VersionAlreadyDeleted(Guid versionId);
    void VersionAlreadyReleased(Guid versionId);
    void VersionDeleted(Guid versionId);
    void Conflict(Conflict conflict);
}