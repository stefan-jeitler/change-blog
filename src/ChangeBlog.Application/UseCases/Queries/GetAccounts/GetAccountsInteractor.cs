using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Accounts;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Extensions;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Version;

namespace ChangeBlog.Application.UseCases.Queries.GetAccounts;

public class GetAccountsInteractor : IGetAccounts, IGetAccount
{
    private readonly IAccountDao _accountDao;
    private readonly IUserDao _userDao;
    private readonly IVersioningSchemeDao _versioningSchemeDao;

    public GetAccountsInteractor(IAccountDao accountDao, IUserDao userDao, IVersioningSchemeDao versioningSchemeDao)
    {
        _accountDao = accountDao ?? throw new ArgumentNullException(nameof(accountDao));
        _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        _versioningSchemeDao = versioningSchemeDao ?? throw new ArgumentNullException(nameof(versioningSchemeDao));
    }

    public async Task<AccountResponseModel> ExecuteAsync(Guid userId, Guid accountId)
    {
        var user = await _userDao.GetUserAsync(userId);
        var account = await _accountDao.GetAccountAsync(accountId);

        var schemeId = account.DefaultVersioningSchemeId;
        var scheme = schemeId.HasValue
            ? await _versioningSchemeDao.GetSchemeAsync(schemeId.Value)
            : await _versioningSchemeDao.GetSchemeAsync(Defaults.VersioningSchemeId);

        return CreateResponseModel(account, scheme, user);
    }

    public async Task<IList<AccountResponseModel>> ExecuteAsync(Guid userId)
    {
        var user = await _userDao.GetUserAsync(userId);
        var accounts = await _accountDao.GetAccountsAsync(userId);

        var schemeIds = accounts
            .Where(x => x.DefaultVersioningSchemeId.HasValue)
            .Select(x => x.DefaultVersioningSchemeId.Value)
            .Distinct()
            .ToList();

        var versioningScheme = await _versioningSchemeDao.GetSchemesAsync(schemeIds);
        var schemeById = versioningScheme.ToDictionary(x => x.Id, x => x);
        var defaultScheme = await _versioningSchemeDao.GetSchemeAsync(Defaults.VersioningSchemeId);

        return accounts.Select(x =>
            {
                var scheme = schemeById.GetValueOrDefault(x.DefaultVersioningSchemeId ?? Guid.Empty, defaultScheme);
                return CreateResponseModel(x, scheme, user);
            })
            .ToList();
    }

    private static AccountResponseModel CreateResponseModel(Account account, VersioningScheme scheme, User user)
    {
        return new(account.Id,
            account.Name,
            scheme.Name,
            scheme.Id,
            account.CreatedAt.ToLocal(user.TimeZone));
    }
}