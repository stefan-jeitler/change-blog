using System;

namespace ChangeTracker.Application.UseCases.Command.AssignPendingLineToVersion.Models
{
    public class VersionIdAssignmentRequestModel
    {
        public VersionIdAssignmentRequestModel(Guid versionId, Guid changeLogLineId)
        {
            if (versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;

            if (changeLogLineId == Guid.Empty)
                throw new ArgumentException("ChangeLogLineId cannot be empty");

            ChangeLogLineId = changeLogLineId;
        }
        
        public Guid VersionId { get; }
        public Guid ChangeLogLineId { get; }
    }
}