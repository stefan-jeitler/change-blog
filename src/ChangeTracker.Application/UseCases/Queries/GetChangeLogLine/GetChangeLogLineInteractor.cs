using System;
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

        public Task<Maybe<ChangeLogLineResponseModel>> ExecuteAsync(Guid userId, Guid changeLogLineId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            if (changeLogLineId == Guid.Empty)
                throw new ArgumentException("ChangeLogLineId cannot be empty.");

            return FindChangeLogLineAsync(userId, changeLogLineId);
        }

        private async Task<Maybe<ChangeLogLineResponseModel>> FindChangeLogLineAsync(Guid userId, Guid changeLogLineId)
        {
            var currentUser = await _userDao.GetUserAsync(userId);

            var line = await _changeLogQueries.FindLineAsync(changeLogLineId);

            if (line.HasNoValue)
                return Maybe<ChangeLogLineResponseModel>.None;

            if(line.Value.IsPending)
                return Maybe<ChangeLogLineResponseModel>.None;

            var l = line.Value;
            var responseModel = new ChangeLogLineResponseModel(l.Id,
                l.Text, 
                l.Labels.Select(ll => ll.Value).ToList(),
                l.Issues.Select(i => i.Value).ToList(),
                l.CreatedAt.ToLocal(currentUser.TimeZone));

            return Maybe<ChangeLogLineResponseModel>.From(responseModel);
        }
    }
}