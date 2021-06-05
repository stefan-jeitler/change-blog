﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.ChangeLog;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Application.UseCases.Queries.SharedModels;

namespace ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogs
{
    public class GetPendingChangeLogLinesInteractor : IGetPendingChangeLogLines
    {
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IProductDao _productDao;
        private readonly IUserDao _userDao;

        public GetPendingChangeLogLinesInteractor(IChangeLogQueriesDao changeLogQueries, IUserDao userDao,
            IProductDao productDao)
        {
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
            _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
        }

        public async Task<PendingChangeLogsResponseModel> ExecuteAsync(Guid userId, Guid productId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            return await GetChangeLogsAsync(userId, productId);
        }

        private async Task<PendingChangeLogsResponseModel> GetChangeLogsAsync(Guid userId, Guid productId)
        {
            var currentUser = await _userDao.GetUserAsync(userId);
            var product = await _productDao.GetProductAsync(productId);

            var pendingChangeLogs = await _changeLogQueries.GetChangeLogsAsync(productId);

            var lines = pendingChangeLogs
                .Lines
                .Select(x => new ChangeLogLineResponseModel(x.Id,
                x.Text,
                x.Labels.Select(l => l.Value).ToList(),
                x.Issues.Select(i => i.Value).ToList(),
                x.CreatedAt.ToLocal(currentUser.TimeZone)
            )).ToList();


            return new PendingChangeLogsResponseModel(product.Id,
                product.Name,
                product.AccountId, 
                lines);
        }
    }
}