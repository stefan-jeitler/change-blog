using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetCompleteVersions
{
    public interface ISearchCompleteVersions
    {
        Task<IList<CompleteVersionResponseModel>> ExecuteAsync(VersionsQueryRequestModel requestModel);
    }
}