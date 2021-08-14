using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Common;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.DataAccess.Products
{
    public interface IProductDao
    {
        Task<Maybe<Product>> FindProductAsync(Guid accountId, Name name);
        Task<Maybe<Product>> FindProductAsync(Guid productId);

        Task<Product> GetProductAsync(Guid productId);
        Task<IList<Product>> GetAccountProductsAsync(AccountProductsQuerySettings querySettings);
        Task<IList<Product>> GetUserProductsAsync(UserProductsQuerySettings querySettings);

        Task<Result<Product, Conflict>> AddProductAsync(Product product);
        Task CloseProductAsync(Product product);

        Task<IList<Name>> GetSupportedLanguageCodesAsync();
    }
}
