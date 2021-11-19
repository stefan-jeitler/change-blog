using System;
using ChangeBlog.Application.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.DeleteVersion;

public interface IDeleteVersionOutputPort
{
    void VersionDoesNotExist(Guid versionId);
    void RelatedProductClosed(Guid productId);
    void VersionAlreadyDeleted(Guid versionId);
    void VersionAlreadyReleased(Guid versionId);
    void VersionDeleted(Guid versionId);
    void Conflict(Conflict conflict);
}