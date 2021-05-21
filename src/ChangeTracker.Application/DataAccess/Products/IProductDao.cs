using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Products
{
    public interface IProductDao
    {
        Task<Maybe<Product>> FindProductAsync(Guid accountId, Name name);
        Task<Maybe<Product>> FindProductAsync(Guid productId);

        Task<Product> GetProductAsync(Guid productId);
        Task<IList<Product>> GetAccountProductsAsync(AccountProductsQuerySettings querySettings);
        Task<IList<Product>> GetUserProductsAsync(UserProductsQuerySettings querySettings);

        Task<Result<Product, Conflict>> AddProductAsync(Product newProduct);
        Task CloseProductAsync(Product product);
    }
}