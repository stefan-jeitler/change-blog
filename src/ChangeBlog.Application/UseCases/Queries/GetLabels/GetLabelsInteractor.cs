using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess.ChangeLog;

namespace ChangeBlog.Application.UseCases.Queries.GetLabels
{
    public class GetLabelsInteractor : IGetLabels
    {
        private readonly IChangeLogQueriesDao _changeLogQueries;

        public GetLabelsInteractor(IChangeLogQueriesDao changeLogQueries)
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

            return line.GetValueOrThrow().Labels
                .Select(x => x.Value)
                .ToList();
        }
    }
}
