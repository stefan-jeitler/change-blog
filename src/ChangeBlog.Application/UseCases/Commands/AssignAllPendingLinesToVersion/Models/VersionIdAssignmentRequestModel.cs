using System;

namespace ChangeBlog.Application.UseCases.Commands.AssignAllPendingLinesToVersion.Models
{
    public class VersionIdAssignmentRequestModel
    {
        public VersionIdAssignmentRequestModel(Guid productId, Guid versionId)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            ProductId = productId;

            if (versionId == Guid.Empty)
                throw new ArgumentException("TargetVersionId cannot be empty.");

            VersionId = versionId;
        }

        public Guid ProductId { get; }
        public Guid VersionId { get; }
    }
}
