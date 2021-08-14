using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.DataAccess.Versions
{
    public interface IVersionDao
    {
        Task<Maybe<ClVersion>> FindVersionAsync(Guid productId, ClVersionValue versionValue);
        Task<Maybe<ClVersion>> FindVersionAsync(Guid versionId);
        Task<Maybe<ClVersion>> FindLatestAsync(Guid productId);
        Task<ClVersion> GetVersionAsync(Guid versionId);
        Task<IList<ClVersion>> GetVersionsAsync(VersionQuerySettings querySettings);
        Task<Result<ClVersion, Conflict>> AddVersionAsync(ClVersion version);
        Task<Result<ClVersion, Conflict>> DeleteVersionAsync(ClVersion version);
        Task<Result<ClVersion, Conflict>> ReleaseVersionAsync(ClVersion version);
        Task<Result<ClVersion, Conflict>> UpdateVersionAsync(ClVersion version);
    }
}
