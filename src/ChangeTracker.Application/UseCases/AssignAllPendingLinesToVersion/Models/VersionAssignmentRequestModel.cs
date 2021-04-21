using System;

namespace ChangeTracker.Application.UseCases.AssignAllPendingLinesToVersion.Models
{
    public class VersionAssignmentRequestModel
    {
        public VersionAssignmentRequestModel(Guid projectId, string version)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.");

            ProjectId = projectId;
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        public Guid ProjectId { get; }
        public string Version { get; }
    }
}