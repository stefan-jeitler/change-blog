﻿using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLog;
using ChangeTracker.Application.UseCases.Commands.Labels.SharedModels;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Commands.Labels.DeleteChangeLogLineLabel
{
    public class DeleteChangeLogLineLabelInteractor : IDeleteChangeLogLineLabel
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteChangeLogLineLabelInteractor(IUnitOfWork unitOfWork, IChangeLogQueriesDao changeLogQueries,
            IChangeLogCommandsDao changeLogCommands)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        }

        public async Task ExecuteAsync(IDeleteChangeLogLineLabelOutputPort output,
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

        private async Task RemoveLabelAsync(IDeleteChangeLogLineLabelOutputPort output, ChangeLogLine line, Label label)
        {
            line.RemoveLabel(label);

            await _changeLogCommands.UpdateLineAsync(line)
                .Match(Finish, output.Conflict);

            void Finish(ChangeLogLine l)
            {
                _unitOfWork.Commit();
                output.Deleted(l.Id);
            }
        }
    }
}