using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class VersionDaoMock : IVersionDao
    {
        public List<ClVersion> Versions { get; } = new();
        public bool ProduceConflict { get; set; }

        public Task<Maybe<ClVersion>> FindAsync(Guid projectId, ClVersionValue versionValue)
        {
            var v = Versions.TryFirst(x => x.ProjectId == projectId && x.Value == versionValue);

            return Task.FromResult(v);
        }

        public Task<Maybe<ClVersion>> FindAsync(Guid projectId, Guid versionId)
        {
            return Task.FromResult(Versions.TryFirst(x => x.ProjectId == projectId && x.Id == versionId));
        }

        public Task<Result<ClVersion, Conflict>> AddAsync(ClVersion clVersion)
        {
            if (ProduceConflict)
            {
                var conflict = Result.Failure<ClVersion, Conflict>(new Conflict("some conflict"));
                return Task.FromResult(conflict);
            }

            Versions.Add(clVersion);
            return Task.FromResult(Result.Success<ClVersion, Conflict>(clVersion));
        }
    }
}