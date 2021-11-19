using System;
using System.Threading.Tasks;
using ChangeBlog.Domain.Authorization;

namespace ChangeBlog.Application.UseCases.Queries.GetAuthorizationState;

public interface IGetAuthorizationState
{
    Task<AuthorizationState> GetAuthStateByAccountIdAsync(Guid userId, Guid accountId, Permission permission);
    Task<AuthorizationState> GetAuthStateByProductIdAsync(Guid userId, Guid productId, Permission permission);
    Task<AuthorizationState> GetAuthStateByVersionIdAsync(Guid userId, Guid versionId, Permission permission);
    Task<AuthorizationState> GetAuthStateByChangeLogLineIdAsync(Guid userId, Guid changeLogLineId, Permission permission);
}