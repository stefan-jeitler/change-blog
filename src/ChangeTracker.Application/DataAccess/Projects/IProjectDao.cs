using System;
using System.Threading.Tasks;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Projects
{
    public interface IProjectDao
    {
        Task<Maybe<Project>> FindProjectAsync(Guid accountId, Name name);
        Task<Maybe<Project>> FindProjectAsync(Guid projectId);
        Task<Project> GetProjectAsync(Guid projectId);
        Task<Result<Project, Conflict>> AddProjectAsync(Project newProject);
        Task<Result<Project, Conflict>> DeleteProjectAsync(Project project);
    }
}