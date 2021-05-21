using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Domain.Common;
using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using static ChangeTracker.DataAccess.Postgres.DataAccessObjects.Product.ProductDaoSqlStatements;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.Product
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

        public async Task<Maybe<Domain.Product>> FindProductAsync(Guid accountId, Name name)
        {
            var product = await _dbAccessor.DbConnection
                .QueryFirstOrDefaultAsync<Domain.Product>(FindProductByAccountAndNameSql, new
                {
                    accountId,
                    name = name.Value.ToLower()
                });

            return product == default
                ? Maybe<Domain.Product>.None
                : Maybe<Domain.Product>.From(product);
        }

        public async Task<Maybe<Domain.Product>> FindProductAsync(Guid productId)
        {
            var product = await _dbAccessor.DbConnection
                .QueryFirstOrDefaultAsync<Domain.Product>(FindProductByProductIdSql, new
                {
                    productId
                });

            return product == default
                ? Maybe<Domain.Product>.None
                : Maybe<Domain.Product>.From(product);
        }

        public async Task<Domain.Product> GetProductAsync(Guid productId)
        {
            var product = await FindProductAsync(productId);

            if (product.HasNoValue)
                throw new Exception(
                    "The requested product does not exist. If you are not sure whether the product exists use 'FindProduct' otherwise file an issue.");

            return product.Value;
        }

        public async Task<IList<Domain.Product>> GetAccountProductsAsync(AccountProductsQuerySettings querySettings)
        {
            var sql = GetProductsForAccountSql(querySettings.LastProductId.HasValue,
                querySettings.IncludeClosedProducts);

            var products = await _dbAccessor.DbConnection
                .QueryAsync<Domain.Product>(sql, new
                {
                    querySettings.AccountId,
                    querySettings.UserId,
                    permission = Permission.ViewAccountProducts.ToString(),
                    querySettings.LastProductId,
                    limit = (int) querySettings.Limit
                });

            return products.AsList();
        }

        public async Task<IList<Domain.Product>> GetUserProductsAsync(UserProductsQuerySettings querySettings)
        {
            var sql = GetProductsForUserSql(querySettings.LastProductId.HasValue,
                querySettings.IncludeClosedProducts);

            var products = await _dbAccessor.DbConnection
                .QueryAsync<Domain.Product>(sql, new
                {
                    querySettings.UserId,
                    permission = Permission.ViewUserProducts.ToString(),
                    querySettings.LastProductId,
                    limit = (int) querySettings.Limit
                });

            return products.AsList();
        }

        public async Task<Result<Domain.Product, Conflict>> AddProductAsync(Domain.Product newProduct)
        {
            const string insertProductSql = @"
                    INSERT INTO product (id, account_id, versioning_scheme_id, name, created_by_user, closed_at, created_at)
                    VALUES (@id, @accountId, @versioningSchemeId, @name, @user, @closedAt, @createdAt)";

            try
            {
                await _dbAccessor.DbConnection
                    .ExecuteAsync(insertProductSql, new
                    {
                        id = newProduct.Id,
                        accountId = newProduct.AccountId,
                        versioningSchemeId = newProduct.VersioningScheme.Id,
                        name = newProduct.Name,
                        user = newProduct.CreatedByUser,
                        closedAt = newProduct.ClosedAt,
                        createdAt = newProduct.CreatedAt
                    });

                return Result.Success<Domain.Product, Conflict>(newProduct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task CloseProductAsync(Domain.Product product)
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
    }
}