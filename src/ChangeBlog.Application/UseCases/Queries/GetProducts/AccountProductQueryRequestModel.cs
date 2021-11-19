using System;

namespace ChangeBlog.Application.UseCases.Queries.GetProducts;

public class AccountProductQueryRequestModel
{
    public const ushort MaxLimit = 100;

    public AccountProductQueryRequestModel(Guid userId, Guid accountId, Guid? lastProductId, ushort limit,
        bool includeClosedProducts)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.");

        UserId = userId;

        if (accountId == Guid.Empty)
            throw new ArgumentException("AccountId cannot be empty.");

        AccountId = accountId;
        LastProductId = lastProductId;
        Limit = Math.Min(limit, MaxLimit);
        IncludeClosedProducts = includeClosedProducts;
    }

    public Guid UserId { get; }
    public Guid AccountId { get; }
    public Guid? LastProductId { get; }
    public ushort Limit { get; }
    public bool IncludeClosedProducts { get; }
}