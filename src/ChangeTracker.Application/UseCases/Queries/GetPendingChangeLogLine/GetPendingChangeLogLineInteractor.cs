﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.ChangeLog;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Application.UseCases.Queries.SharedModels;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogLine
{
    public class GetPendingChangeLogLineInteractor : IGetPendingChangeLogLine
    {
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IProductDao _productDao;
        private readonly IUserDao _userDao;

        public GetPendingChangeLogLineInteractor(IChangeLogQueriesDao changeLogQueries, IUserDao userDao,
            IProductDao productDao)
        {
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
            _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
        }

        public async Task ExecuteAsync(IGetPendingChangeLogLineOutputPort output,
            Guid userId, Guid changeLogLineId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            if (changeLogLineId == Guid.Empty)
                throw new ArgumentException("ChangeLogLineId cannot be empty.");

            await GetChangeLogLineAsync(output, userId, changeLogLineId);
        }

        private async Task GetChangeLogLineAsync(
            IGetPendingChangeLogLineOutputPort output,
            Guid userId,
            Guid changeLogLineId)
        {
            var line = await _changeLogQueries.FindLineAsync(changeLogLineId);

            if (line.HasNoValue)
            {
                output.LineDoesNotExists(changeLogLineId);
            }

            if (!line.Value.IsPending)
            {
                output.LineIsNotPending(changeLogLineId);
            }

            var currentUser = await _userDao.GetUserAsync(userId);
            var product = await _productDao.GetProductAsync(line.Value.ProductId);

            var l = line.Value;
            var lineResponseModel = new ChangeLogLineResponseModel(
                l.Id,
                l.Text,
                l.Labels.Select(ll => ll.Value).ToList(),
                l.Issues.Select(i => i.Value).ToList(),
                l.CreatedAt.ToLocal(currentUser.TimeZone));

            output.LineFound(new PendingChangeLogLineResponseModel(product.Id,
                product.Name,
                product.AccountId,
                lineResponseModel));
        }
    }
}