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

        public async Task<Result<Project, Conflict>> CloseProjectAsync(Project project)
        {
            await Task.Yield();

            if (ProduceConflict)
            {
                return Result.Failure<Project, Conflict>(new Conflict("something went badly wrong."));
            }

            Projects.RemoveAll(x => x.Id == project.Id);
            return Result.Success<Project, Conflict>(project);
        }
    }
}