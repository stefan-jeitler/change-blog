using System;
using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Queries.SharedModels;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Queries.GetVersions;

public interface IGetVersion
{
    Task<Maybe<VersionResponseModel>> ExecuteAsync(Guid userId, Guid versionId);
    Task<Maybe<VersionResponseModel>> ExecuteAsync(Guid userId, Guid productId, string version);
}