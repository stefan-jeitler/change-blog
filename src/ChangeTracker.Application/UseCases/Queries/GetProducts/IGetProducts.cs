using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetProducts
{
    public interface IGetProducts
    {
        Task<IList<ProductResponseModel>> ExecuteAsync(ProductQueryRequestModel requestModel);

        Task<Maybe<ProductResponseModel>> ExecuteAsync(Guid userId, Guid productId);
    }
}