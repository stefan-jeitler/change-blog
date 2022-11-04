using System;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.Products.GetProducts;

public class AccountProductQueryRequestModel
{
    public const ushort MaxLimit = 100;

    public AccountProductQueryRequestModel(Guid userId, Guid accountId, Guid? lastProductId, ushort limit,
        bool includeFreezedProducts)
    {
        UserId = Guard.Against.NullOrEmpty(userId, nameof(userId));
        AccountId = Guard.Against.NullOrEmpty(accountId, nameof(accountId));
        LastProductId = lastProductId;
        Limit = Math.Min(limit, MaxLimit);
        IncludeFreezedProducts = includeFreezedProducts;
    }

    public Guid UserId { get; }
    public Guid AccountId { get; }
    public Guid? LastProductId { get; }
    public ushort Limit { get; }
    public bool IncludeFreezedProducts { get; }
}