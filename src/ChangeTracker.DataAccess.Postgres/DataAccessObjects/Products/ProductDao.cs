﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using static ChangeTracker.DataAccess.Postgres.DataAccessObjects.Products.ProductDaoSqlStatements;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.Products
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
                    querySettings.AccountId,
                    querySettings.UserId,
                    permission = Permission.ViewAccountProducts.ToString(),
                    querySettings.LastProductId,
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
                    querySettings.UserId,
                    permission = Permission.ViewUserProducts.ToString(),
                    querySettings.LastProductId,
                    limit = (int) querySettings.Limit
                });

            return products.AsList();
        }

        public async Task<Result<Product, Conflict>> AddProductAsync(Product product)
        {
            const string insertProductSql = @"
                    INSERT INTO product (id, account_id, versioning_scheme_id, name, created_by_user, closed_at, created_at)
                    VALUES (@id, @accountId, @versioningSchemeId, @name, @user, @closedAt, @createdAt)";

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
                        createdAt = product.CreatedAt
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

        public async Task<Result<Product, Conflict>> UpdateProductAsync(Product product)
        {
            try
            {
                const string closeProductSql =
                    "UPDATE product SET name = @name, versioning_scheme_id @versioningSchemeId WHERE id = @productId";

                await _dbAccessor.DbConnection.ExecuteScalarAsync(closeProductSql, new
                {
                    name = product.Name,
                    versioningSchemeId = product.VersioningScheme.Id,
                    productId = product.Id
                });

                return Result.Success<Product, Conflict>(product);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }
        }
    }
}