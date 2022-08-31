using System;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Extensions;
using ChangeBlog.Application.UseCases.SharedModels;

namespace ChangeBlog.Application.UseCases.ChangeLogs.GetChangeLogLine;

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
        Guard.Against.NullOrEmpty(userId, nameof(userId));
        Guard.Against.NullOrEmpty(changeLogLineId, nameof(changeLogLineId));

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

        if (line.GetValueOrThrow().IsPending)
        {
            output.LineIsPending(changeLogLineId);
            return;
        }

        var l = line.GetValueOrThrow();
        var responseModel = new ChangeLogLineResponseModel(l.Id,
            l.Text,
            l.Labels.Select(ll => ll.Value).ToList(),
            l.Issues.Select(i => i.Value).ToList(),
            l.CreatedAt.ToLocal(currentUser.TimeZone));

        output.LineFound(responseModel);
    }
}