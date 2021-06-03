using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetVersions
{
    public interface IGetVersions
    {
        Task<IList<VersionResponseModel>> ExecuteAsync(VersionsQueryRequestModel requestModel);
    }
}