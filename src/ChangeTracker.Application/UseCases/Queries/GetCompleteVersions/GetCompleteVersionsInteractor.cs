using System;
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
    public class GetCompleteVersionsInteractor : IGetCompleteVersions
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

            var responseModel = CreateResponseModel(clVersion, product, currentUser.TimeZone, changeLogs);
            return Maybe<CompleteVersionResponseModel>.From(responseModel);
        }

        private static CompleteVersionResponseModel CreateResponseModel(Maybe<ClVersion> clVersion, Product product,
            string timeZone, ChangeLogs changeLogs)
        {
            return new(clVersion.Value.Id,
                clVersion.Value.ProductId,
                product.Name,
                product.AccountId,
                clVersion.Value.CreatedAt.ToLocal(timeZone),
                changeLogs.Lines.Select(x =>
                        new ChangeLogLineResponseModel(x.Id,
                            x.Text,
                            x.Labels.Select(l => l.Value).ToList(),
                            x.Issues.Select(i => i.Value).ToList(),
                            x.CreatedAt.ToLocal(timeZone)))
                    .ToList()
            );
        }
    }
}