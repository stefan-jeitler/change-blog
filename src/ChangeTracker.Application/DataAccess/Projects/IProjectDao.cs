using System;
using System.Collections.Generic;
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

        /// <summary>
        ///     Loads projects for the given account ordered by name
        /// </summary>
        /// <param name="accountId">AccountId</param>
        /// <param name="count">Loads max count projects</param>
        /// <param name="lastProjectId"></param>
        /// <returns>List of projects ordered by name</returns>
        Task<IList<Project>> GetProjectsAsync(Guid accountId, ushort count, Guid? lastProjectId = null);

        Task<Result<Project, Conflict>> AddProjectAsync(Project newProject);
        Task CloseProjectAsync(Project project);
    }
}