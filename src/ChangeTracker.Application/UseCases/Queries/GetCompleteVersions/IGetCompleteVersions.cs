using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetCompleteVersions
{
    public interface IGetCompleteVersions
    {
        Task<Maybe<CompleteVersionResponseModel>> ExecuteAsync(Guid userId, Guid versionId);
    }
}