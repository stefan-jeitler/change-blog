using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Queries.SharedModels;

namespace ChangeTracker.Application.UseCases.Queries.GetVersions
{
    public interface IGetVersions
    {
        Task<IList<VersionResponseModel>> ExecuteAsync(VersionsQueryRequestModel requestModel);
    }
}