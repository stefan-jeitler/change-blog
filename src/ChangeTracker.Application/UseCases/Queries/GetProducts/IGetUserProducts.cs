using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetProducts
{
    public interface IGetUserProducts
    {
        Task<IList<ProductResponseModel>> ExecuteAsync(UserProductQueryRequestModel requestModel);
    }
}