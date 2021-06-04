using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Commands.DeleteChangeLogLine
{
    public class DeleteChangeLogLineInteractor : IDeleteChangeLogLine
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;

        public DeleteChangeLogLineInteractor(IChangeLogCommandsDao changeLogCommands,
            IChangeLogQueriesDao changeLogQueries)
        {
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
        }

        public async Task ExecuteAsync(IDeleteChangeLogLineOutputPort output, Guid changeLogLineId)
        {
            var existingLine = await _changeLogQueries.FindLineAsync(changeLogLineId);

            if (existingLine.HasNoValue)
            {
                output.LineDoesNotExist(changeLogLineId);
                return;
            }

            if (existingLine.Value.DeletedAt.HasValue)
            {
                output.LineDeleted(existingLine.Value.Id);
                return;
            }

            await _changeLogCommands.DeleteLineAsync(existingLine.Value)
                .Match(
                    l => output.LineDeleted(l.Id),
                    c => output.Conflict(c.Reason));
        }
    }
}