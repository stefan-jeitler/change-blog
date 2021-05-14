using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetProjects
{
    public interface IGetProjects
    {
        Task<IEnumerable<ProjectResponseModel>> ExecuteAsync(ProjectsQueryRequestModel queryRequestModel);

        Task<Maybe<ProjectResponseModel>> ExecuteAsync(Guid userId, Guid projectId);
    }
}