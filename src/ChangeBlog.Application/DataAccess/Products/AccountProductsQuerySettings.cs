using System;

namespace ChangeBlog.Application.DataAccess.Products
{
    public class AccountProductsQuerySettings
    {
        public AccountProductsQuerySettings(Guid accountId, Guid userId, Guid? lastProductId = null, ushort limit = 100,
            bool includeClosedProducts = false)
        {
            AccountId = accountId;
            UserId = userId;
            LastProductId = lastProductId;
            Limit = limit;
            IncludeClosedProducts = includeClosedProducts;
        }

        public Guid UserId { get; }
        public Guid AccountId { get; }
        public Guid? LastProductId { get; }
        public ushort Limit { get; }
        public bool IncludeClosedProducts { get; }
    }
}
