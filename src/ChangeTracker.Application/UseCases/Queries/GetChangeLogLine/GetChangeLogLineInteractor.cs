﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.ChangeLog;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Application.UseCases.Queries.SharedModels;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetChangeLogLine
{
    public class GetChangeLogLineInteractor : IGetChangeLogLine
    {
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUserDao _userDao;

        public GetChangeLogLineInteractor(IChangeLogQueriesDao changeLogQueries, IUserDao userDao)
        {
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
            _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        }

        public Task ExecuteAsync(IGetChangeLogLineOutputPort output, Guid userId, Guid changeLogLineId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            if (changeLogLineId == Guid.Empty)
                throw new ArgumentException("ChangeLogLineId cannot be empty.");

            return FindChangeLogLineAsync(output, userId, changeLogLineId);
        }

        private async Task FindChangeLogLineAsync(IGetChangeLogLineOutputPort output, Guid userId, Guid changeLogLineId)
        {
            var currentUser = await _userDao.GetUserAsync(userId);

            var line = await _changeLogQueries.FindLineAsync(changeLogLineId);

            if (line.HasNoValue)
            {
                output.LineDoesNotExists(changeLogLineId);
                return;
            }

            if (line.Value.IsPending)
            {
                output.LineIsPending(changeLogLineId);
                return;
            }

            var l = line.Value;
            var responseModel = new ChangeLogLineResponseModel(l.Id,
                l.Text,
                l.Labels.Select(ll => ll.Value).ToList(),
                l.Issues.Select(i => i.Value).ToList(),
                l.CreatedAt.ToLocal(currentUser.TimeZone));

            output.LineFound(responseModel);
        }
    }
}