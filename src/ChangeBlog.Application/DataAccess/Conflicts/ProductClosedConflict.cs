using System;

namespace ChangeBlog.Application.DataAccess.Conflicts;

public class ProductClosedConflict : Conflict
{
    public ProductClosedConflict(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty.");

        ProductId = productId;
    }

    public Guid ProductId { get; }
}