using System;
using ChangeBlog.Application.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.ReleaseVersion;

public interface IReleaseVersionOutputPort
{
    void VersionAlreadyReleased(Guid versionId);
    void VersionAlreadyDeleted(Guid versionId);
    void VersionReleased(Guid versionId);
    void Conflict(Conflict conflict);
    void VersionDoesNotExist(Guid versionId);
    void RelatedProductClosed(Guid productId);
}