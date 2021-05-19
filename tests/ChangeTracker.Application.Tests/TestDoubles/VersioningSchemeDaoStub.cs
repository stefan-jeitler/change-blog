using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class VersioningSchemeDaoStub : IVersioningSchemeDao
    {
        public List<VersioningScheme> VersioningSchemes { get; } = new();

        public async Task<Maybe<VersioningScheme>> FindSchemeAsync(Guid versioningSchemeId)
        {
            await Task.Yield();

            return VersioningSchemes.TryFirst(x => x.Id == versioningSchemeId);
        }

        public async Task<VersioningScheme> GetSchemeAsync(Guid versioningSchemeId)
        {
            await Task.Yield();

            return VersioningSchemes.Single(x => x.Id == versioningSchemeId);
        }

        public async Task<IList<VersioningScheme>> GetSchemesAsync(IList<Guid> versioningSchemeIds)
        {
            await Task.Yield();

            return VersioningSchemes
                .Where(x => versioningSchemeIds.Any(y => x.Id == y))
                .ToList();
        }
    }
}