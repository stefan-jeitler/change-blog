using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.SharedModels;

namespace ChangeBlog.Application.UseCases.Versions.GetVersions;

public interface IGetVersions
{
    Task<IList<VersionResponseModel>> ExecuteAsync(VersionsQueryRequestModel requestModel);
}