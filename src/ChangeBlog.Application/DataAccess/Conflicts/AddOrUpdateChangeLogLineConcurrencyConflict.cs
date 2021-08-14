using System;

namespace ChangeBlog.Application.DataAccess.Conflicts
{
    public class AddOrUpdateChangeLogLineConcurrencyConflict : Conflict
    {
        public AddOrUpdateChangeLogLineConcurrencyConflict(Guid productId, Guid? versionId, Guid changeLogLineId)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            ProductId = productId;

            if (versionId.HasValue && versionId.Value == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;

            if (changeLogLineId == Guid.Empty)
                throw new ArgumentException("ChangeLogLineId cannot be empty.");

            ChangeLogLineId = changeLogLineId;
        }

        public Guid ProductId { get; }
        public Guid? VersionId { get; }
        public Guid ChangeLogLineId { get; }
    }
}
