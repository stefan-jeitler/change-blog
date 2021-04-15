using System;

namespace ChangeTracker.Application.UseCases.AssignPendingLineToVersion.Models
{
    public class VersionIdAssignmentRequestModel
    {
        public VersionIdAssignmentRequestModel(Guid projectId, Guid versionId, Guid changeLogLineId)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.");

            ProjectId = projectId;

            if (versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;

            if (changeLogLineId == Guid.Empty)
                throw new ArgumentException("ChangeLogLineId cannot be empty");

            ChangeLogLineId = changeLogLineId;
        }

        public Guid ProjectId { get; }
        public Guid VersionId { get; }
        public Guid ChangeLogLineId { get; }
    }
}