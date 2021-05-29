﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class ProductDaoStub : IProductDao
    {
        public List<Product> Products { get; } = new();
        public bool ProduceConflict { get; set; }

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

        public Task<Result<Product, Conflict>> AddProductAsync(Product newProduct)
        {
            if (ProduceConflict)
            {
                var conflict = new Conflict("some conflict");
                return Task.FromResult(Result.Failure<Product, Conflict>(conflict));
            }

            Products.Add(newProduct);
            return Task.FromResult(Result.Success<Product, Conflict>(newProduct));
        }

        public Task CloseProductAsync(Product product)
        {
            Products.RemoveAll(x => x.Id == product.Id);
            Products.Add(product);
            return Task.CompletedTask;
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
}