using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.DataAccess.Versions;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.Tests.TestDoubles
{
    public class VersionDaoStub : IVersionDao
    {
        public List<ClVersion> Versions { get; } = new();
        public Conflict Conflict { get; set; }

        public Task<Maybe<ClVersion>> FindVersionAsync(Guid productId, ClVersionValue versionValue)
        {
            var version = Versions.TryFirst(x => x.ProductId == productId && x.Value == versionValue);

            return Task.FromResult(version);
        }

        public Task<Maybe<ClVersion>> FindVersionAsync(Guid versionId)
        {
            return Task.FromResult(Versions.TryFirst(x => x.Id == versionId));
        }

        public Task<Maybe<ClVersion>> FindLatestAsync(Guid productId)
        {
            return Task.FromResult(Versions.OrderByDescending(x => x.CreatedAt).TryFirst());
        }

        public Task<ClVersion> GetVersionAsync(Guid versionId)
        {
            return Task.FromResult(Versions.Single(x => x.Id == versionId));
        }

        /// <summary>
        ///     Not properly implemented, but should be enough for use-case tests
        ///     The actual implementation of IVersionDao is tested separately.
        /// </summary>
        /// <param name="querySettings"></param>
        /// <returns></returns>
        public async Task<IList<ClVersion>> GetVersionsAsync(VersionQuerySettings querySettings)
        {
            await Task.Yield();

            return Versions
                .Where(x => x.ProductId == querySettings.ProductId)
                .Where(x => querySettings.IncludeDeleted || !x.IsDeleted)
                .Take(querySettings.Limit)
                .ToList();
        }

        public Task<Result<ClVersion, Conflict>> AddVersionAsync(ClVersion clVersion)
        {
            if (Conflict is not null)
            {
                var conflict = Result.Failure<ClVersion, Conflict>(Conflict);
                return Task.FromResult(conflict);
            }

            Versions.Add(clVersion);
            return Task.FromResult(Result.Success<ClVersion, Conflict>(clVersion));
        }

        public async Task<Result<ClVersion, Conflict>> DeleteVersionAsync(ClVersion version)
        {
            await Task.Yield();

            if (Conflict is not null)
            {
                return Result.Failure<ClVersion, Conflict>(Conflict);
            }

            Versions.RemoveAll(x => x.Id == version.Id);
            return Result.Success<ClVersion, Conflict>(version);
        }

        public async Task<Result<ClVersion, Conflict>> ReleaseVersionAsync(ClVersion version)
        {
            await Task.Yield();

            if (Conflict is not null)
            {
                return Result.Failure<ClVersion, Conflict>(Conflict);
            }

            Versions.RemoveAll(x => x.Id == version.Id);
            Versions.Add(version);
            return Result.Success<ClVersion, Conflict>(version);
        }

        public async Task<Result<ClVersion, Conflict>> UpdateVersionAsync(ClVersion version)
        {
            await Task.Yield();

            if (Conflict is not null)
            {
                return Result.Failure<ClVersion, Conflict>(Conflict);
            }

            Versions.RemoveAll(x => x.Id == version.Id);
            Versions.Add(version);

            return Result.Success<ClVersion, Conflict>(version);
        }
    }
}
