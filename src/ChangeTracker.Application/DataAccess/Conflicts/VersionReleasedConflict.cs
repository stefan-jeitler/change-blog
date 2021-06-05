using System;

namespace ChangeTracker.Application.DataAccess.Conflicts
{
    public class VersionReleasedConflict : Conflict
    {
        public VersionReleasedConflict(Guid versionId)
        {
            if (versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;
        }

        public Guid VersionId { get; }
    }
}