using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.DataAccess.Products;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Common;
using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using static ChangeBlog.DataAccess.Postgres.DataAccessObjects.Products.ProductDaoSqlStatements;

namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Products
{
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

            return product.Value;
        }

        public async Task<IList<Product>> GetAccountProductsAsync(AccountProductsQuerySettings querySettings)
        {
            var sql = GetProductsForAccountSql(querySettings.LastProductId.HasValue,
                querySettings.IncludeClosedProducts);

            var products = await _dbAccessor.DbConnection
                .QueryAsync<Product>(sql, new
                {
                    accountId = querySettings.AccountId,
                    userId = querySettings.UserId,
                    lastProductId = querySettings.LastProductId,
                    limit = (int) querySettings.Limit
                });

            return products.AsList();
        }

        public async Task<IList<Product>> GetUserProductsAsync(UserProductsQuerySettings querySettings)
        {
            var sql = GetProductsForUserSql(querySettings.LastProductId.HasValue,
                querySettings.IncludeClosedProducts);

            var products = await _dbAccessor.DbConnection
                .QueryAsync<Product>(sql, new
                {
                    userId = querySettings.UserId,
                    lastProductId = querySettings.LastProductId,
                    limit = (int) querySettings.Limit
                });

            return products.AsList();
        }

        public async Task<Result<Product, Conflict>> AddProductAsync(Product product)
        {
            const string insertProductSql = @"
                INSERT INTO product (id, account_id, versioning_scheme_id, name, created_by_user, closed_at, created_at, language_code)
                VALUES (@id, @accountId, @versioningSchemeId, @name, @user, @closedAt, @createdAt, @languageCode)";

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
                        closedAt = product.ClosedAt,
                        createdAt = product.CreatedAt,
                        languageCode = product.LanguageCode.Value
                    });

                return Result.Success<Product, Conflict>(product);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task CloseProductAsync(Product product)
        {
            if (!product.ClosedAt.HasValue)
                throw new Exception("The given product has no closed date.");

            const string closeProductSql = "UPDATE product SET closed_at = @closedAt WHERE id = @productId";

            await _dbAccessor.DbConnection.ExecuteScalarAsync(closeProductSql, new
            {
                closedAt = product.ClosedAt,
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
}