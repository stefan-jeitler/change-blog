using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Products.GetProducts;

public interface IGetUserProducts
{
    Task<IList<ProductResponseModel>> ExecuteAsync(UserProductQueryRequestModel requestModel);
}