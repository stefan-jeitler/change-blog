using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetVersions
{
    public interface IGetVersion
    {
        Task<Maybe<VersionResponseModel>> ExecuteAsync(Guid userId, Guid versionId);
        Task<Maybe<VersionResponseModel>> ExecuteAsync(Guid userId, Guid productId, string version);
    }
}