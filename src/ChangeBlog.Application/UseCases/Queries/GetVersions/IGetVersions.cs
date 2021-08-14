using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Queries.SharedModels;

namespace ChangeBlog.Application.UseCases.Queries.GetVersions
{
    public interface IGetVersions
    {
        Task<IList<VersionResponseModel>> ExecuteAsync(VersionsQueryRequestModel requestModel);
    }
}
