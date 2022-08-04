using System;

namespace ChangeBlog.Application.UseCases.Products.GetProducts;

public class UserProductQueryRequestModel
{
    public const ushort MaxLimit = 100;

    public UserProductQueryRequestModel(Guid userId, Guid? lastProductId,
        ushort limit, bool includeClosedProducts)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.");
        }

        UserId = userId;

        LastProductId = lastProductId;
        Limit = Math.Min(limit, MaxLimit);
        IncludeClosedProducts = includeClosedProducts;
    }

    public Guid UserId { get; }
    public Guid? LastProductId { get; }
    public ushort Limit { get; }
    public bool IncludeClosedProducts { get; }
}