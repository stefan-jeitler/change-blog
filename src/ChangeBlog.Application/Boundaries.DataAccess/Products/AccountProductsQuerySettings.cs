using System;

namespace ChangeBlog.Application.Boundaries.DataAccess.Products;

public class AccountProductsQuerySettings
{
    public AccountProductsQuerySettings(Guid accountId, Guid userId, Guid? lastProductId = null, ushort limit = 100,
        bool includeFreezedProducts = false)
    {
        AccountId = accountId;
        UserId = userId;
        LastProductId = lastProductId;
        Limit = limit;
        IncludeFreezedProducts = includeFreezedProducts;
    }

    public Guid UserId { get; }
    public Guid AccountId { get; }
    public Guid? LastProductId { get; }
    public ushort Limit { get; }
    public bool IncludeFreezedProducts { get; }
}