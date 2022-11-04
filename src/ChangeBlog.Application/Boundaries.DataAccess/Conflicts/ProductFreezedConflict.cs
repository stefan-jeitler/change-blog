using System;

namespace ChangeBlog.Application.Boundaries.DataAccess.Conflicts;

public class ProductFreezedConflict : Conflict
{
    public ProductFreezedConflict(Guid productId)
    {
        if (productId == Guid.Empty) throw new ArgumentException("ProductId cannot be empty.");

        ProductId = productId;
    }

    public Guid ProductId { get; }
}