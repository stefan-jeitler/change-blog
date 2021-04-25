using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class VersionDaoStub : IVersionDao
    {
        public List<ClVersion> Versions { get; } = new();
        public bool ProduceConflict { get; set; }

        public Task<Maybe<ClVersion>> FindVersionAsync(Guid projectId, ClVersionValue versionValue)
        {
            var version = Versions.TryFirst(x => x.ProjectId == projectId && x.Value == versionValue);

            return Task.FromResult(version);
        }

        public Task<Maybe<ClVersion>> FindVersionAsync(Guid versionId)
        {
            return Task.FromResult(Versions.TryFirst(x => x.Id == versionId));
        }

        public Task<ClVersion> GetVersionAsync(Guid versionId)
        {
            return Task.FromResult(Versions.Single(x => x.Id == versionId));
        }

        public Task<Result<ClVersion, Conflict>> AddVersionAsync(ClVersion clVersion)
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