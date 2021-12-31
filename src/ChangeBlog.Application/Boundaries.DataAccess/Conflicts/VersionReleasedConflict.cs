using System;

namespace ChangeBlog.Application.Boundaries.DataAccess.Conflicts;

public class VersionReleasedConflict : Conflict
{
    public VersionReleasedConflict(Guid versionId)
    {
        if (versionId == Guid.Empty)
        {
            throw new ArgumentException("VersionId cannot be empty.");
        }

        VersionId = versionId;
    }

    public Guid VersionId { get; }
}