using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;
using OneOf;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class VersionDaoMock : IVersionDao
    {
        public List<ClVersionInfo> VersionInfo { get; } = new();
        public bool ProduceConflict { get; set; }

        public Task<Maybe<ClVersionInfo>> FindAsync(Guid projectId, ClVersion version)
        {
            var v = VersionInfo.TryFirst(x => x.ProjectId == projectId && x.Value == version);

            return Task.FromResult(v);
        }

        public Task<OneOf<ClVersionInfo, Conflict>> AddAsync(ClVersionInfo clVersion)
        {
            if (ProduceConflict)
            {
                var conflict = OneOf<ClVersionInfo, Conflict>.FromT1(new Conflict("some conflict"));
                return Task.FromResult(conflict);
            }

            VersionInfo.Add(clVersion);
            return Task.FromResult(OneOf<ClVersionInfo, Conflict>.FromT0(clVersion));
        }
    }
}