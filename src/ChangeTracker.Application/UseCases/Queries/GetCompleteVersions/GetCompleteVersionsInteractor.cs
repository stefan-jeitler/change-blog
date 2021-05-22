using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Application.UseCases.Queries.SharedModels;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetCompleteVersions
{
    public class GetCompleteVersionsInteractor : IGetCompleteVersion, ISearchCompleteVersions
    {
        private readonly IChangeLogQueriesDao _changeLogQueriesDao;
        private readonly IProductDao _productDao;
        private readonly IUserDao _userDao;
        private readonly IVersionDao _versionDao;

        public GetCompleteVersionsInteractor(IProductDao productDao, IVersionDao versionDao,
            IChangeLogQueriesDao changeLogQueriesDao, IUserDao userDao)
        {
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _changeLogQueriesDao = changeLogQueriesDao ?? throw new ArgumentNullException(nameof(changeLogQueriesDao));
            _userDao = userDao;
        }

        public async Task<Maybe<CompleteVersionResponseModel>> ExecuteAsync(Guid userId, Guid versionId)
        {
            var clVersion = await _versionDao.FindVersionAsync(versionId);
            if (clVersion.HasNoValue)
                return Maybe<CompleteVersionResponseModel>.None;

            var currentUser = await _userDao.GetUserAsync(userId);
            var product = await _productDao.GetProductAsync(clVersion.Value.ProductId);

            var changeLogs =
                await _changeLogQueriesDao.GetChangeLogsAsync(clVersion.Value.ProductId, clVersion.Value.Id);

            var responseModel = CreateResponseModel(clVersion.Value, product, currentUser.TimeZone, changeLogs);
            return Maybe<CompleteVersionResponseModel>.From(responseModel);
        }

        public async Task<IList<CompleteVersionResponseModel>> ExecuteAsync(VersionsQueryRequestModel requestModel)
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
                    changeLogsByVersionId[x.Id]))
                .ToList();

            return null;
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

        private static CompleteVersionResponseModel CreateResponseModel(ClVersion clVersion, Product product,
            string timeZone, ChangeLogs changeLogs)
        {
            return new(clVersion.Id,
                clVersion.Value.Value,
                clVersion.ProductId,
                product.Name,
                product.AccountId,
                changeLogs.Lines.Select(x =>
                        new ChangeLogLineResponseModel(x.Id,
                            x.Text,
                            x.Labels.Select(l => l.Value).ToList(),
                            x.Issues.Select(i => i.Value).ToList(),
                            x.CreatedAt.ToLocal(timeZone)))
                    .ToList(),
                clVersion.CreatedAt.ToLocal(timeZone),
                clVersion.ReleasedAt?.ToLocal(timeZone)
            );
        }
    }
}