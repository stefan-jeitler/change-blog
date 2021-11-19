using System;

namespace ChangeBlog.Application.DataAccess.Conflicts;

public class ChangeLogLineDeletedConflict : Conflict
{
    public ChangeLogLineDeletedConflict(Guid changeLogLineId)
    {
        if (changeLogLineId == Guid.Empty)
            throw new ArgumentException("VersionId cannot be empty.");

        ChangeLogLineId = changeLogLineId;
    }

    public Guid ChangeLogLineId { get; }
}