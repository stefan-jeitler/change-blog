using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.UseCases.Command.Labels.SharedModels;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Command.Labels.RemoveChangeLogLineLabel
{
    public class RemoveChangeLogLineLabelInteractor : IRemoveChangeLogLineLabel
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveChangeLogLineLabelInteractor(IUnitOfWork unitOfWork, IChangeLogQueriesDao changeLogQueries,
            IChangeLogCommandsDao changeLogCommands)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        }

        public async Task ExecuteAsync(IRemoveChangeLogLineLabelOutputPort output,
            ChangeLogLineLabelRequestModel requestModel)
        {
            if (!Label.TryParse(requestModel.Label, out var label))
            {
                output.InvalidLabel(requestModel.Label);
                return;
            }

            _unitOfWork.Start();

            var line = await _changeLogQueries.FindLineAsync(requestModel.ChangeLogLineId);
            if (line.HasNoValue)
            {
                output.ChangeLogLineDoesNotExist();
                return;
            }

            await RemoveLabelAsync(output, line.Value, label);
        }

        private async Task RemoveLabelAsync(IRemoveChangeLogLineLabelOutputPort output, ChangeLogLine line, Label label)
        {
            line.RemoveLabel(label);

            await _changeLogCommands.UpdateLineAsync(line)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(ChangeLogLine l)
            {
                _unitOfWork.Commit();
                output.Removed(l.Id);
            }
        }
    }
}