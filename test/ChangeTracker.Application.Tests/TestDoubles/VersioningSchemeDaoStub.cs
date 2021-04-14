using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class VersioningSchemeDaoStub : IVersioningSchemeDao
    {
        public VersioningScheme VersioningScheme { get; set; }

        public Task<Maybe<VersioningScheme>> FindAsync(Guid versioningSchemeId) =>
            versioningSchemeId == VersioningScheme?.Id
                ? Task.FromResult(Maybe<VersioningScheme>.From(VersioningScheme))
                : Task.FromResult(Maybe<VersioningScheme>.None);
    }
}