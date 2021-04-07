using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using CSharpFunctionalExtensions;
using OneOf;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class ProjectDaoMock : IProjectDao
    {
        public List<Project> Projects { get; set; } = new();
        public bool ProduceConflict { get; set; }

        public Task<Maybe<Project>> FindAsync(Guid accountId, Name name)
        {
            var project = Projects.TryFirst(x => x.AccountId == accountId
                                                 && x.Name == name);

            return Task.FromResult(project);
        }

        public Task<Maybe<Project>> FindAsync(Guid projectId) =>
            Task.FromResult(Projects.TryFirst(x => x.Id == projectId));

        public Task<OneOf<Project, Conflict>> AddAsync(Project newProject)
        {
            if (ProduceConflict)
            {
                var conflict = new Conflict("some conflict");
                return Task.FromResult(OneOf<Project, Conflict>.FromT1(conflict));
            }

            Projects.Add(newProject);
            return Task.FromResult(OneOf<Project, Conflict>.FromT0(newProject));
        }
    }
}