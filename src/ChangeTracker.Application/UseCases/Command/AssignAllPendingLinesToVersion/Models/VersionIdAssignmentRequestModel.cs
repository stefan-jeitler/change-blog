using System;

namespace ChangeTracker.Application.UseCases.Command.AssignAllPendingLinesToVersion.Models
{
    public class VersionIdAssignmentRequestModel
    {
        public VersionIdAssignmentRequestModel(Guid projectId, Guid versionId)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.");

            ProjectId = projectId;

            if (versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;
        }

        public Guid ProjectId { get; }
        public Guid VersionId { get; }
    }
}