using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.ChangeLogs;
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
                output.LineDoesNotExist();
                return;
            }

            var deletedLine = existingLine.Value.Delete();
            await _changeLogCommands.DeleteLineAsync(deletedLine)
                .Match(
                    l => output.LineDeleted(l.Id),
                    c => output.Conflict(c.Reason));
        }
    }
}