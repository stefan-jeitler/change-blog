using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.ChangeLog;

namespace ChangeTracker.Application.UseCases.Queries.GetIssues
{
    public class GetIssuesInteractor : IGetIssues
    {
        private readonly IChangeLogQueriesDao _changeLogQueries;

        public GetIssuesInteractor(IChangeLogQueriesDao changeLogQueries)
        {
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
        }

        public async Task<IList<string>> ExecuteAsync(Guid changeLogLineId)
        {
            if (changeLogLineId == Guid.Empty)
            {
                throw new ArgumentException("ChangeLogLineId cannot be empty.");
            }

            var line = await _changeLogQueries.FindLineAsync(changeLogLineId);

            if (line.HasNoValue)
                return Array.Empty<string>();

            return line.Value.Issues
                .Select(x => x.Value)
                .ToList();
        }
    }
}