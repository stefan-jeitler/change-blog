using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetProjects
{
    public interface IGetProjects
    {
        Task<IEnumerable<ProjectsQueryResponseModel>> ExecuteAsync(ProjectsQueryRequestModel queryRequestModel);
    }
}
