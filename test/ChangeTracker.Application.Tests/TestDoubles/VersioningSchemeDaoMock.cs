using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Domain.ChangeLogVersion;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class VersioningSchemeDaoMock : IVersioningSchemeDao
    {
        public VersioningSchemeDaoMock(VersioningScheme versioningScheme)
        {
            VersioningScheme = versioningScheme;
        }

        public VersioningSchemeDaoMock()
        {
        }

        public VersioningScheme VersioningScheme { get; set; }

        public Task<Maybe<VersioningScheme>> FindAsync(Guid versioningSchemeId) =>
            versioningSchemeId == VersioningScheme?.Id
                ? Task.FromResult(Maybe<VersioningScheme>.From(VersioningScheme))
                : Task.FromResult(Maybe<VersioningScheme>.None);
    }
}