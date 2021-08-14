using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Queries.GetProducts
{
    public interface IGetProduct
    {
        Task<Maybe<ProductResponseModel>> ExecuteAsync(Guid userId, Guid productId);
    }
}
