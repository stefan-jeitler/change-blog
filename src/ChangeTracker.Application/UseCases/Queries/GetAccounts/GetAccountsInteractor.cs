using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Accounts;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Domain;

namespace ChangeTracker.Application.UseCases.Queries.GetAccounts
{
    public class GetAccountsInteractor : IGetAccounts
    {
        private readonly IAccountDao _accountDao;
        private readonly IUserDao _userDao;
        private readonly IVersioningSchemeDao _versioningSchemeDao;

        public GetAccountsInteractor(IAccountDao accountDao, IUserDao userDao, IVersioningSchemeDao versioningSchemeDao)
        {
            _accountDao = accountDao;
            _userDao = userDao;
            _versioningSchemeDao = versioningSchemeDao;
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
                    return new AccountResponseModel(x.Id,
                        x.Name,
                        scheme.Name,
                        scheme.Id,
                        x.CreatedAt.ToLocal(user.TimeZone));
                })
                .ToList();
        }

        public async Task<AccountResponseModel> ExecuteAsync(Guid userId, Guid accountId)
        {
            var user = await _userDao.GetUserAsync(userId);
            var account = await _accountDao.GetAccountAsync(accountId);

            var schemeId = account.DefaultVersioningSchemeId;
            var scheme = schemeId.HasValue
                ? await _versioningSchemeDao.GetSchemeAsync(schemeId.Value)
                : await _versioningSchemeDao.GetSchemeAsync(Defaults.VersioningSchemeId);

            return new AccountResponseModel(account.Id,
                account.Name,
                scheme.Name,
                scheme.Id,
                account.CreatedAt.ToLocal(user.TimeZone));
        }
    }
}