using System;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess.Users;
using ChangeBlog.Domain.Authorization;

namespace ChangeBlog.Application.UseCases.Queries.GetAuthorizationState;

public class GetAuthorizationStateInteractor : IGetAuthorizationState
{
    private readonly IUserAccessDao _userAccessDao;

    public GetAuthorizationStateInteractor(IUserAccessDao userAccessDao)
    {
        _userAccessDao = userAccessDao ?? throw new ArgumentNullException(nameof(userAccessDao));
    }

    public async Task<AuthorizationState> GetAuthStateByAccountIdAsync(Guid userId, Guid accountId, Permission permission)
    {
        var accountPermissions = await _userAccessDao.GetAccountRolesAsync(accountId, userId);

        var authService = new AuthorizationService(accountPermissions);

        return authService.GetAuthorizationState(permission);
    }

    public async Task<AuthorizationState> GetAuthStateByProductIdAsync(Guid userId, Guid productId, Permission permission)
    {
        var (account, product) = await _userAccessDao.GetRolesByProductIdAsync(userId, productId);

        var authService = new AuthorizationService(account, product);

        return authService.GetAuthorizationState(permission);
    }

    public async Task<AuthorizationState> GetAuthStateByVersionIdAsync(Guid userId, Guid versionId, Permission permission)
    {
        var (account, product) = await _userAccessDao.GetRolesByVersionIdAsync(userId, versionId);

        var authService = new AuthorizationService(account, product);

        return authService.GetAuthorizationState(permission);
    }

    public async Task<AuthorizationState> GetAuthStateByChangeLogLineIdAsync(Guid userId, Guid changeLogLineId, Permission permission)
    {
        var (account, product) = await _userAccessDao.GetRolesByChangeLogLineIdAsync(userId, changeLogLineId);

        var authService = new AuthorizationService(account, product);

        return authService.GetAuthorizationState(permission);
    }
}