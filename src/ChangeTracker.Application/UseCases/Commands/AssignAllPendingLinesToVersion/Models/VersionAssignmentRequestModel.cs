using System;

namespace ChangeTracker.Application.UseCases.Commands.AssignAllPendingLinesToVersion.Models
{
    public class VersionAssignmentRequestModel
    {
        public VersionAssignmentRequestModel(Guid productId, string version)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            ProductId = productId;
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        public Guid ProductId { get; }
        public string Version { get; }
    }
}