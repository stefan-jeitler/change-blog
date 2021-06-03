using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.ChangeLog;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Application.UseCases.Queries.SharedModels;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetVersions
{
    public class GetVersionsInteractor : IGetVersion, IGetVersions
    {
        private readonly IChangeLogQueriesDao _changeLogQueriesDao;
        private readonly IProductDao _productDao;
        private readonly IUserDao _userDao;
        private readonly IVersionDao _versionDao;

        public GetVersionsInteractor(IProductDao productDao, IVersionDao versionDao,
            IChangeLogQueriesDao changeLogQueriesDao, IUserDao userDao)
        {
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _changeLogQueriesDao = changeLogQueriesDao ?? throw new ArgumentNullException(nameof(changeLogQueriesDao));
            _userDao = userDao;
        }

        public async Task<Maybe<VersionResponseModel>> ExecuteAsync(Guid userId, Guid versionId)
        {
            var clVersion = await _versionDao.FindVersionAsync(versionId);
            if (clVersion.HasNoValue)
                return Maybe<VersionResponseModel>.None;

            var currentUser = await _userDao.GetUserAsync(userId);
            var product = await _productDao.GetProductAsync(clVersion.Value.ProductId);

            var changeLogs =
                await _changeLogQueriesDao.GetChangeLogsAsync(clVersion.Value.ProductId, clVersion.Value.Id);

            var responseModel = CreateResponseModel(clVersion.Value, product, currentUser.TimeZone, changeLogs);
            return Maybe<VersionResponseModel>.From(responseModel);
        }

        public async Task<Maybe<VersionResponseModel>> ExecuteAsync(Guid userId, Guid productId, string version)
        {
            if (!ClVersionValue.TryParse(version, out var clVersionValue))
            {
                return Maybe<VersionResponseModel>.None;
            }

            var clVersion = await _versionDao.FindVersionAsync(productId, clVersionValue);
            if (clVersion.HasNoValue)
                return Maybe<VersionResponseModel>.None;

            var currentUser = await _userDao.GetUserAsync(userId);
            var product = await _productDao.GetProductAsync(clVersion.Value.ProductId);

            var changeLogs =
                await _changeLogQueriesDao.GetChangeLogsAsync(clVersion.Value.ProductId, clVersion.Value.Id);

            var responseModel = CreateResponseModel(clVersion.Value, product, currentUser.TimeZone, changeLogs);
            return Maybe<VersionResponseModel>.From(responseModel);
        }

        public async Task<IList<VersionResponseModel>> ExecuteAsync(VersionsQueryRequestModel requestModel)
        {
            var querySettings = new VersionQuerySettings(requestModel.ProductId,
                requestModel.LastVersionId,
                requestModel.SearchTerm,
                requestModel.Limit,
                requestModel.IncludeDeleted);

            var currentUser = await _userDao.GetUserAsync(requestModel.UserId);
            var product = await _productDao.GetProductAsync(requestModel.ProductId);

            var versions = await _versionDao.GetVersionsAsync(querySettings);
            var changeLogsByVersionId = await GetChangeLogsAsync(versions);

            return versions
                .Select(x => CreateResponseModel(x,
                    product,
                    currentUser.TimeZone,
                    changeLogsByVersionId.GetValueOrDefault(x.Id, new ChangeLogs(new ChangeLogLine[0]))))
                .ToList();
        }

        private async Task<IReadOnlyDictionary<Guid, ChangeLogs>> GetChangeLogsAsync(IEnumerable<ClVersion> clVersions)
        {
            var versionIds = clVersions
                .Select(x => x.Id)
                .Distinct()
                .ToList();

            var changeLogs = await _changeLogQueriesDao.GetChangeLogsAsync(versionIds);

            return changeLogs
                .ToDictionary(x => x.VersionId!.Value, x => x);
        }

        private static VersionResponseModel CreateResponseModel(ClVersion clVersion, Product product,
            string timeZone, ChangeLogs changeLogs)
        {
            return new(clVersion.Id,
                clVersion.Value.Value,
                clVersion.Name.Value,
                clVersion.ProductId,
                product.Name,
                product.AccountId,
                changeLogs.Lines
                    .OrderBy(x => x.Position)
                    .Select(x =>
                        new ChangeLogLineResponseModel(x.Id,
                            x.Text,
                            x.Labels.Select(l => l.Value).ToList(),
                            x.Issues.Select(i => i.Value).ToList(),
                            x.CreatedAt.ToLocal(timeZone)))
                    .ToList(),
                clVersion.CreatedAt.ToLocal(timeZone),
                clVersion.ReleasedAt?.ToLocal(timeZone),
                clVersion.DeletedAt?.ToLocal(timeZone)
            );
        }
    }
}