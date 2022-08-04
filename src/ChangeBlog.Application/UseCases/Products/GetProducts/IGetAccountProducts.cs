using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Products.GetProducts;

public interface IGetAccountProducts
{
    Task<IList<ProductResponseModel>> ExecuteAsync(AccountProductQueryRequestModel requestModel);
}