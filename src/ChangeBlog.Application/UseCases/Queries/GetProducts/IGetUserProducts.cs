using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Queries.GetProducts
{
    public interface IGetUserProducts
    {
        Task<IList<ProductResponseModel>> ExecuteAsync(UserProductQueryRequestModel requestModel);
    }
}
