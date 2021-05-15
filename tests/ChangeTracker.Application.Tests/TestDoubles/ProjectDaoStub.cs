using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class ProjectDaoStub : IProjectDao
    {
        public List<Project> Projects { get; set; } = new();
        public bool ProduceConflict { get; set; }

        public Task<Maybe<Project>> FindProjectAsync(Guid accountId, Name name)
        {
            var project = Projects.TryFirst(x => x.AccountId == accountId
                                                 && x.Name == name);

            return Task.FromResult(project);
        }

        public Task<Maybe<Project>> FindProjectAsync(Guid projectId) =>
            Task.FromResult(Projects.TryFirst(x => x.Id == projectId));

        public async Task<Project> GetProjectAsync(Guid projectId)
        {
            await Task.Yield();
            return Projects.Single(x => x.Id == projectId);
        }

        public async Task<IList<Project>> GetProjectsAsync(ProjectQuerySettings querySettings)
        {
            await Task.Yield();

            var lastEmail = Projects.FirstOrDefault(x => x.Id == querySettings.LastProjectId);

            return Projects
                .Where(x => x.AccountId == querySettings.AccountId)
                .OrderBy(x => x.Name.Value)
                .Where(x => lastEmail is null || string.Compare(x.Name, lastEmail.Name) > 0 )
                .Take(querySettings.Count)
                .ToList();
        }

        public async Task<IList<Project>> GetProjectsAsync(Guid accountId, ushort count, Guid? lastProjectId = null)
        {
            await Task.Yield();

            return Projects.Where(x => x.AccountId == accountId)
                .OrderBy(x => x.Name)
                .SkipWhile(x => x.Id != (lastProjectId ?? Guid.Empty))
                .Skip(1)
                .Take(count)
                .ToList();
        }

        public Task<Result<Project, Conflict>> AddProjectAsync(Project newProject)
        {
            if (ProduceConflict)
            {
                var conflict = new Conflict("some conflict");
                return Task.FromResult(Result.Failure<Project, Conflict>(conflict));
            }

            Projects.Add(newProject);
            return Task.FromResult(Result.Success<Project, Conflict>(newProject));
        }

        public Task CloseProjectAsync(Project project)
        {
            Projects.RemoveAll(x => x.Id == project.Id);
            return Task.CompletedTask;
        }
    }
}