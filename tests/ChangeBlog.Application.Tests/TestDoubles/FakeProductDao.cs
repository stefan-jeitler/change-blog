using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.Tests.TestDoubles;

public class FakeProductDao : IProductDao
{
    public List<Product> Products { get; } = new();
    public Conflict Conflict { get; set; }

    public Task<Maybe<Product>> FindProductAsync(Guid accountId, Name name)
    {
        var product = Products.TryFirst(x => x.AccountId == accountId
                                             && x.Name == name);

        return Task.FromResult(product);
    }

    public Task<Maybe<Product>> FindProductAsync(Guid productId)
    {
        return Task.FromResult(Products.TryFirst(x => x.Id == productId));
    }

    public async Task<Product> GetProductAsync(Guid productId)
    {
        await Task.Yield();
        return Products.Single(x => x.Id == productId);
    }

    public async Task<IList<Product>> GetAccountProductsAsync(AccountProductsQuerySettings querySettings)
    {
        await Task.Yield();

        var lastEmail = Products.FirstOrDefault(x => x.Id == querySettings.LastProductId);

        return Products
            .Where(x => x.AccountId == querySettings.AccountId)
            .OrderBy(x => x.Name.Value)
            .Where(x => lastEmail is null || string.Compare(x.Name, lastEmail.Name) > 0)
            .Take(querySettings.Limit)
            .ToList();
    }

    public Task<Result<Product, Conflict>> AddProductAsync(Product product)
    {
        if (Conflict is not null) return Task.FromResult(Result.Failure<Product, Conflict>(Conflict));

        Products.Add(product);
        return Task.FromResult(Result.Success<Product, Conflict>(product));
    }

    public Task CloseProductAsync(Product product)
    {
        Products.RemoveAll(x => x.Id == product.Id);
        Products.Add(product);
        return Task.CompletedTask;
    }

    public Task<IList<Name>> GetSupportedLanguageCodesAsync()
    {
        return Task.FromResult((IList<Name>)new List<Name> { Name.Parse("en"), Name.Parse("de") });
    }

    /// <summary>
    ///     Not properly implemented, but should be enough for use-case tests
    ///     The actual implementation of IProductDao is tested separately.
    /// </summary>
    /// <param name="querySettings"></param>
    /// <returns></returns>
    public async Task<IList<Product>> GetUserProductsAsync(UserProductsQuerySettings querySettings)
    {
        await Task.Yield();

        var lastEmail = Products.FirstOrDefault(x => x.Id == querySettings.LastProductId);

        return Products
            .OrderBy(x => x.Name.Value)
            .Where(x => lastEmail is null || string.Compare(x.Name, lastEmail.Name) > 0)
            .Take(querySettings.Limit)
            .ToList();
    }
}