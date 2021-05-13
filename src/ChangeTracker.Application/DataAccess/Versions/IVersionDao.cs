using System;
using System.Threading.Tasks;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Versions
{
    public interface IVersionDao
    {
        Task<Maybe<ClVersion>> FindVersionAsync(Guid projectId, ClVersionValue versionValue);
        Task<Maybe<ClVersion>> FindVersionAsync(Guid versionId);
        Task<ClVersion> GetVersionAsync(Guid versionId);
        Task<Result<ClVersion, Conflict>> AddVersionAsync(ClVersion version);
        Task<Result<ClVersion, Conflict>> DeleteVersionAsync(ClVersion version);
    }
}