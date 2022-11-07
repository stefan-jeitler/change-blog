using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;

namespace ChangeBlog.Application.UseCases.ChangeLogs.ChangeLogLineExists;

public class ChangeLogLineExistsInteractor : IChangeLogLineExists
{
    private readonly IChangeLogQueriesDao _changeLogQueriesDao;

    public ChangeLogLineExistsInteractor(IChangeLogQueriesDao changeLogQueriesDao)
    {
        _changeLogQueriesDao = changeLogQueriesDao;
    }

    public async Task<bool> ExecuteAsync(Guid changeLogLineId)
    {
        var changeLogLine = await _changeLogQueriesDao.FindLineAsync(changeLogLineId);

        return changeLogLine.HasValue;
    }
}