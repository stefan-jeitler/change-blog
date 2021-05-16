using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetProjects
{
    public interface IGetProjects
    {
        Task<IList<ProjectResponseModel>> ExecuteAsync(ProjectsQueryRequestModel requestModel);

        Task<Maybe<ProjectResponseModel>> ExecuteAsync(Guid userId, Guid projectId);
    }
}