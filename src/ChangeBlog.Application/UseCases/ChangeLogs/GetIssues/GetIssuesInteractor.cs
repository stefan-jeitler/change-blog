using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;

namespace ChangeBlog.Application.UseCases.ChangeLogs.GetIssues;

public class GetIssuesInteractor : IGetIssues
{
    private readonly IChangeLogQueriesDao _changeLogQueries;

    public GetIssuesInteractor(IChangeLogQueriesDao changeLogQueries)
    {
        _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
    }

    public async Task<IList<string>> ExecuteAsync(Guid changeLogLineId)
    {
        Guard.Against.NullOrEmpty(changeLogLineId, nameof(changeLogLineId));

        var line = await _changeLogQueries.FindLineAsync(changeLogLineId);

        if (line.HasNoValue)
        {
            return Array.Empty<string>();
        }

        return line.GetValueOrThrow().Issues
            .Select(x => x.Value)
            .ToList();
    }
}