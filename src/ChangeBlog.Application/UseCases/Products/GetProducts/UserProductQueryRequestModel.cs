using System;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.Products.GetProducts;

public class UserProductQueryRequestModel
{
    public const ushort MaxLimit = 100;

    public UserProductQueryRequestModel(Guid userId, Guid? lastProductId,
        string nameFilter, ushort limit, bool includeFreezedProducts)
    {
        UserId = Guard.Against.NullOrEmpty(userId, nameof(userId));
        LastProductId = lastProductId;
        NameFilter = nameFilter;
        Limit = Math.Min(limit, MaxLimit);
        IncludeFreezedProducts = includeFreezedProducts;
    }

    public Guid UserId { get; }
    public Guid? LastProductId { get; }
    public string NameFilter { get; }
    public ushort Limit { get; }
    public bool IncludeFreezedProducts { get; }
}