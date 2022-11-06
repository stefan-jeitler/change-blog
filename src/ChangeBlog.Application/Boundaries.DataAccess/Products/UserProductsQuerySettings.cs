using System;

namespace ChangeBlog.Application.Boundaries.DataAccess.Products;

public class UserProductsQuerySettings
{
    public UserProductsQuerySettings(Guid userId, Guid? lastProductId = null, ushort limit = 100,
        bool includeFreezedProducts = false)
    {
        UserId = userId;
        LastProductId = lastProductId;
        Limit = limit;
        IncludeFreezedProducts = includeFreezedProducts;
    }

    public Guid UserId { get; }
    public Guid? LastProductId { get; }
    public ushort Limit { get; }
    public bool IncludeFreezedProducts { get; }
}