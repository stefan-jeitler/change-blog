using System;

namespace ChangeTracker.Application.UseCases.AssignPendingLineToVersion.Models
{
    public class VersionAssignmentRequestModel
    {
        public VersionAssignmentRequestModel(Guid projectId, string version, Guid changeLogLineId)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.");

            ProjectId = projectId;
            Version = version ?? throw new ArgumentNullException(nameof(version));

            if (changeLogLineId == Guid.Empty)
                throw new ArgumentException("LineId cannot be empty.");

            ChangeLogLineId = changeLogLineId;
        }

        public Guid ProjectId { get; }
        public string Version { get; }
        public Guid ChangeLogLineId { get; }
    }
}