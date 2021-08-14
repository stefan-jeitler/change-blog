using System;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess.ChangeLog;
using CSharpFunctionalExtensions;

// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

namespace ChangeBlog.Application.UseCases.Commands.DeleteChangeLogLine
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

        public async Task ExecuteAsync(IDeleteChangeLogLineOutputPort output,
            DeleteChangeLogLineRequestModel requestModel)
        {
            var existingLine = await _changeLogQueries.FindLineAsync(requestModel.ChangeLogLineId);

            if (existingLine.HasNoValue)
            {
                output.LineDoesNotExist(requestModel.ChangeLogLineId);
                return;
            }

            switch (requestModel.ChangeLogLineType)
            {
                case ChangeLogLineType.Pending when !existingLine.Value.IsPending:
                    output.RequestedLineIsNotPending(existingLine.Value.Id);
                    return;
                case ChangeLogLineType.NotPending when existingLine.Value.IsPending:
                    output.RequestedLineIsPending(existingLine.Value.Id);
                    return;
            }

            if (existingLine.Value.DeletedAt.HasValue)
            {
                output.LineDeleted(existingLine.Value.Id);
                return;
            }

            await _changeLogCommands.DeleteLineAsync(existingLine.Value)
                .Match(l => output.LineDeleted(l.Id), output.Conflict);
        }
    }
}
