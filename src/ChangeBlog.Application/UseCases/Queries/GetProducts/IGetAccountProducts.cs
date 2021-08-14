using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Queries.GetProducts
{
    public interface IGetAccountProducts
    {
        Task<IList<ProductResponseModel>> ExecuteAsync(AccountProductQueryRequestModel requestModel);
    }
}
