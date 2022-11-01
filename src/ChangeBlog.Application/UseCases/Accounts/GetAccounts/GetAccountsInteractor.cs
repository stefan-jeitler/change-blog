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

namespace ChangeBlog.Application.UseCases.Accounts.GetAccounts;

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
        var currentUser = await _userDao.GetUserAsync(userId);
        var account = await _accountDao.GetAccountAsync(accountId);

        var schemeId = account.DefaultVersioningSchemeId;
        var scheme = schemeId.HasValue
            ? await _versioningSchemeDao.GetSchemeAsync(schemeId.Value)
            : await _versioningSchemeDao.GetSchemeAsync(Default.VersioningSchemeId);

        var createdBy = await _userDao.GetUserAsync(account.CreatedByUser);

        return CreateResponseModel(account, scheme, createdBy, currentUser);
    }

    public async Task<IList<AccountResponseModel>> ExecuteAsync(Guid userId)
    {
        var currentUser = await _userDao.GetUserAsync(userId);
        var accounts = await _accountDao.GetAccountsAsync(userId);

        var schemeById = await GetRelevantVersioningSchemesAsync(accounts);
        var defaultScheme = await _versioningSchemeDao.GetSchemeAsync(Default.VersioningSchemeId);

        var creatorById = await GetRelevantUsersAsync(accounts);

        return accounts.Select(x =>
            {
                var scheme = schemeById.GetValueOrDefault(x.DefaultVersioningSchemeId ?? Guid.Empty, defaultScheme);
                var createdBy = creatorById.GetValueOrDefault(x.CreatedByUser);
                return CreateResponseModel(x, scheme, createdBy, currentUser);
            })
            .ToList();
    }

    private async Task<IReadOnlyDictionary<Guid, User>> GetRelevantUsersAsync(IList<Account> accounts)
    {
        var creatorUserIds = accounts
            .Select(x => x.CreatedByUser)
            .Distinct()
            .ToList();
        var creators = await _userDao.GetUsersAsync(creatorUserIds);
        var creatorById = creators.ToDictionary(k => k.Id, v => v);
        return creatorById;
    }

    private async Task<IReadOnlyDictionary<Guid, VersioningScheme>> GetRelevantVersioningSchemesAsync(
        IEnumerable<Account> accounts)
    {
        var schemeIds = accounts
            .Where(x => x.DefaultVersioningSchemeId.HasValue)
            .Select(x => x.DefaultVersioningSchemeId.Value)
            .Distinct()
            .ToList();

        var versioningScheme = await _versioningSchemeDao.GetSchemesAsync(schemeIds);
        return versioningScheme.ToDictionary(x => x.Id, x => x);
    }

    private static AccountResponseModel CreateResponseModel(Account account, VersioningScheme scheme, User createdBy,
        User currentUser) =>
        new(account.Id,
            account.Name,
            scheme.Name,
            scheme.Id,
            createdBy.Email,
            account.CreatedAt.ToLocal(currentUser.TimeZone),
            currentUser.Id == createdBy.Id);
}