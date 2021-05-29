using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetCompleteVersions
{
    public interface IGetCompleteVersions
    {
        Task<IList<CompleteVersionResponseModel>> ExecuteAsync(VersionsQueryRequestModel requestModel);
    }
}