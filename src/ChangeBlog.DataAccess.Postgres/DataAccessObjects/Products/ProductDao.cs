using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using static ChangeBlog.DataAccess.Postgres.DataAccessObjects.Products.ProductDaoSqlStatements;

namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Products;

public class ProductDao : IProductDao
{
    private readonly IDbAccessor _dbAccessor;
    private readonly ILogger<ProductDao> _logger;

    public ProductDao(IDbAccessor dbAccessor, ILogger<ProductDao> logger)
    {
        _dbAccessor = dbAccessor;
        _logger = logger;
    }

    public async Task<Maybe<Product>> FindProductAsync(Guid accountId, Name name)
    {
        var product = await _dbAccessor.DbConnection
            .QueryFirstOrDefaultAsync<Product>(FindProductByAccountAndNameSql, new
            {
                accountId,
                name = name.Value.ToLower()
            });

        return product == default
            ? Maybe<Product>.None
            : Maybe<Product>.From(product);
    }

    public async Task<Maybe<Product>> FindProductAsync(Guid productId)
    {
        var product = await _dbAccessor.DbConnection
            .QueryFirstOrDefaultAsync<Product>(FindProductByProductIdSql, new
            {
                productId
            });

        return product == default
            ? Maybe<Product>.None
            : Maybe<Product>.From(product);
    }

    public async Task<Product> GetProductAsync(Guid productId)
    {
        var product = await FindProductAsync(productId);

        if (product.HasNoValue)
            throw new Exception(
                "The requested product does not exist. If you are not sure whether the product exists use 'FindProduct' otherwise file an issue.");

        return product.GetValueOrThrow();
    }

    public async Task<IList<Product>> GetAccountProductsAsync(AccountProductsQuerySettings querySettings)
    {
        var (activeProductsQuery, activeProductsParameters) =
            new ProductsQueryBuilder(querySettings.UserId, ProductType.Active)
                .AddAccountFilter(querySettings.AccountId)
                .TryAddPagingFilter(querySettings.LastProductId)
                .TryAddNameFilter(querySettings.NameFilter)
                .Build(querySettings.Limit);

        var activeProducts =
            await _dbAccessor.DbConnection.QueryAsync<Product>(activeProductsQuery, activeProductsParameters);

        var activeProductsMaterialized = activeProducts.AsList();
        if (!querySettings.IncludeFreezedProducts)
            return activeProductsMaterialized.AsList();

        var updatedLimit = querySettings.Limit - activeProductsMaterialized.Count;
        if (updatedLimit == 0)
            return activeProductsMaterialized;

        var (freezedProductsSql, freezedProductsParameters) =
            new ProductsQueryBuilder(querySettings.UserId, ProductType.Freezed)
                .AddAccountFilter(querySettings.AccountId)
                .TryAddPagingFilter(querySettings.LastProductId)
                .TryAddNameFilter(querySettings.NameFilter)
                .Build(querySettings.Limit);

        var freezedProducts =
            await _dbAccessor.DbConnection.QueryAsync<Product>(freezedProductsSql, freezedProductsParameters);

        return activeProductsMaterialized.Concat(freezedProducts).AsList();
    }

    public async Task<IList<Product>> GetUserProductsAsync(UserProductsQuerySettings querySettings)
    {
        var (activeProductsQuery, activeProductsParameters) =
            new ProductsQueryBuilder(querySettings.UserId, ProductType.Active)
                .TryAddPagingFilter(querySettings.LastProductId)
                .TryAddNameFilter(querySettings.NameFilter)
                .Build(querySettings.Limit);

        var activeProducts =
            await _dbAccessor.DbConnection.QueryAsync<Product>(activeProductsQuery, activeProductsParameters);

        var activeProductsMaterialized = activeProducts.AsList();
        if (!querySettings.IncludeFreezedProducts)
            return activeProductsMaterialized.AsList();

        var updatedLimit = querySettings.Limit - activeProductsMaterialized.Count;
        if (updatedLimit == 0)
            return activeProductsMaterialized;

        var (freezedProductsSql, freezedProductsParameters) =
            new ProductsQueryBuilder(querySettings.UserId, ProductType.Freezed)
                .TryAddPagingFilter(querySettings.LastProductId)
                .TryAddNameFilter(querySettings.NameFilter)
                .Build(querySettings.Limit);

        var freezedProducts =
            await _dbAccessor.DbConnection.QueryAsync<Product>(freezedProductsSql, freezedProductsParameters);

        return activeProductsMaterialized.Concat(freezedProducts).AsList();
    }

    public async Task<Result<Product, Conflict>> AddProductAsync(Product product)
    {
        const string insertProductSql = @"
                INSERT INTO product (id, account_id, versioning_scheme_id, name, created_by_user, freezed_at, created_at, language_code)
                VALUES (@id, @accountId, @versioningSchemeId, @name, @user, @freezedAt, @createdAt, @languageCode)";

        try
        {
            await _dbAccessor.DbConnection
                .ExecuteAsync(insertProductSql, new
                {
                    id = product.Id,
                    accountId = product.AccountId,
                    versioningSchemeId = product.VersioningScheme.Id,
                    name = product.Name,
                    user = product.CreatedByUser,
                    freezedAt = product.FreezedAt,
                    createdAt = product.CreatedAt,
                    languageCode = product.LanguageCode.Value
                });

            return Result.Success<Product, Conflict>(product);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while adding product");
            throw;
        }
    }

    public async Task FreezeProductAsync(Product product)
    {
        if (!product.FreezedAt.HasValue)
            throw new Exception("The given product has no freezed date.");

        const string freezeProductSql = "UPDATE product SET freezed_at = @freezedAt WHERE id = @productId";

        await _dbAccessor.DbConnection.ExecuteScalarAsync(freezeProductSql, new
        {
            freezedAt = product.FreezedAt,
            productId = product.Id
        });
    }

    public async Task<IList<Name>> GetSupportedLanguageCodesAsync()
    {
        const string getSupportedLangCodesSql = @"
                select code
                from language l
                where exists(SELECT NULL FROM pg_ts_config pt where LOWER(pt.cfgname) = LOWER(l.name))";

        var langCodes = await _dbAccessor.DbConnection.QueryAsync<Name>(getSupportedLangCodesSql);
        return langCodes.AsList();
    }
}