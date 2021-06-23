using System;
using System.Threading.Tasks;
using ChangeTracker.Domain.Authorization;

namespace ChangeTracker.Application.UseCases.Queries.GetAuthorizationState
{
    public interface IGetAuthorizationState
    {
        Task<AuthorizationState> GetAuthStateByAccountIdAsync(Guid userId, Guid accountId, Permission permission);
        Task<AuthorizationState> GetAuthStateByUserAccountsAsync(Guid userId, Permission permission);
        Task<AuthorizationState> GetAuthStateByProductIdAsync(Guid userId, Guid productId, Permission permission);
        Task<AuthorizationState> GetAuthStateByVersionIdAsync(Guid userId, Guid versionId, Permission permission);
        Task<AuthorizationState> GetAuthStateByChangeLogLineIdAsync(Guid userId, Guid changeLogLineId, Permission permission);
    }
}