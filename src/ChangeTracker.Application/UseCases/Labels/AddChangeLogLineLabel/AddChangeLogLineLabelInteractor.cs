using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.UseCases.Labels.SharedModels;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Labels.AddChangeLogLineLabel
{
    public class AddChangeLogLineLabelInteractor : IAddChangeLogLineLabel
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUnitOfWork _unitOfWork;

        public AddChangeLogLineLabelInteractor(IUnitOfWork unitOfWork, IChangeLogQueriesDao changeLogQueries,
            IChangeLogCommandsDao changeLogCommands)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        }

        public async Task ExecuteAsync(IAddChangeLogLineLabelOutputPort output,
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

            if (line.Value.AvailableLabelPlaces <= 0)
            {
                output.MaxLabelsReached(ChangeLogLine.MaxLabels);
                return;
            }

            await AddLabelAsync(output, line.Value, label);
        }

        private async Task AddLabelAsync(IAddChangeLogLineLabelOutputPort output, ChangeLogLine line, Label label)
        {
            line.AddLabel(label);

            await _changeLogCommands.UpdateLineAsync(line)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(ChangeLogLine l)
            {
                _unitOfWork.Commit();
                output.Added(l.Id);
            }
        }
    }
}