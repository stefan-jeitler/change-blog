﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.ChangeLog;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Application.UseCases.Queries.SharedModels;

namespace ChangeTracker.Application.UseCases.Queries.GetLatestVersion
{
    public class GetLatestVersionInteractor : IGetLatestVersion
    {
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IProductDao _productDao;
        private readonly IUserDao _userDao;
        private readonly IVersionDao _versionDao;

        public GetLatestVersionInteractor(IVersionDao versionDao, IChangeLogQueriesDao changeLogQueries,
            IProductDao productDao, IUserDao userDao)
        {
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
            _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        }

        public async Task ExecuteAsync(IGetLatestVersionOutputPort output, Guid userId, Guid productId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("userId cannot be empty.");

            if (productId == Guid.Empty)
                throw new ArgumentException("productId cannot be empty.");

            var currentUser = await _userDao.GetUserAsync(userId);
            var product = await _productDao.FindProductAsync(productId);

            if (product.HasNoValue)
            {
                output.ProductDoesNotExist();
                return;
            }

            var clVersion = await _versionDao.FindLatestAsync(product.Value.Id);

            if (clVersion.HasNoValue)
            {
                output.NoVersionExists(product.Value.Id);
                return;
            }

            var version = clVersion.Value;
            var changeLogLines =
                await _changeLogQueries.GetChangeLogsAsync(version.ProductId, version.Id);

            output.VersionFound(new VersionResponseModel(version.Id,
                version.Value,
                version.Name,
                version.ProductId,
                product.Value.Name,
                product.Value.AccountId,
                changeLogLines.Lines.Select(x => new ChangeLogLineResponseModel(x.Id, x.Text,
                        x.Labels.Select(ll => ll.Value).ToList(),
                        x.Issues.Select(i => i.Value).ToList(),
                        x.CreatedAt.ToLocal(currentUser.TimeZone)))
                    .ToList(),
                version.CreatedAt.ToLocal(currentUser.TimeZone),
                version.ReleasedAt?.ToLocal(currentUser.TimeZone),
                version.DeletedAt?.ToLocal(currentUser.TimeZone)));
        }
    }
}