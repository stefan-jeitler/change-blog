using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetProducts
{
    public interface IGetAccountProducts
    {
        Task<IList<ProductResponseModel>> ExecuteAsync(AccountProductQueryRequestModel requestModel);
    }
}