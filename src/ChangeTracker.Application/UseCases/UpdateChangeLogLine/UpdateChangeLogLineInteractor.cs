﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.UpdateChangeLogLine
{
    public class UpdateChangeLogLineInteractor : IUpdateChangeLogLine
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateChangeLogLineInteractor(IChangeLogQueriesDao changeLogQueries,
            IChangeLogCommandsDao changeLogCommands, IUnitOfWork unitOfWork)
        {
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        }

        public async Task ExecuteAsync(IUpdateLineOutputPort output, ChangeLogLineRequestModel requestModel)
        {
            _unitOfWork.Start();

            var existingLine = await _changeLogQueries.FindLineAsync(requestModel.ChangeLogLineId);
            if (existingLine.HasNoValue)
            {
                output.ChangeLogLineDoesNotExist();
                return;
            }

            var parsedNewLine = ParseLine(output, requestModel);
            if (parsedNewLine.HasNoValue)
                return;

            var lineWithSameTextExists = await LineWithSameTextExists(existingLine, parsedNewLine);
            if (lineWithSameTextExists)
            {
                output.LineWithSameTextAlreadyExists(requestModel.Text);
                return;
            }

            await UpdateLineAsync(output, existingLine.Value, parsedNewLine.Value);
        }

        private async Task<bool> LineWithSameTextExists(Maybe<ChangeLogLine> existingLine, Maybe<LineParserResponseModel> parsedNewLine)
        {
            var changeLogMetadata = await _changeLogQueries.GetChangeLogsMetadataAsync(existingLine.Value.ProjectId,
                existingLine.Value.VersionId);

            return changeLogMetadata.Texts.Contains(parsedNewLine.Value.Text);
        }

        private static Maybe<LineParserResponseModel> ParseLine(ILineParserOutput output,
            ChangeLogLineRequestModel requestModel)
        {
            var lineParsingRequestModel =
                new LineParserRequestModel(requestModel.Text, requestModel.Labels, requestModel.Issues);

            return LineParser.Parse(output, lineParsingRequestModel);
        }

        private async Task UpdateLineAsync(IUpdateLineOutputPort output, ChangeLogLine existingLine,
            LineParserResponseModel updatedValues)
        {
            var line = new ChangeLogLine(existingLine.Id, existingLine.VersionId, existingLine.ProjectId,
                updatedValues.Text, existingLine.Position, existingLine.CreatedAt, updatedValues.Labels,
                updatedValues.Issues, existingLine.DeletedAt);

            await _changeLogCommands.UpdateLineAsync(line)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(ChangeLogLine l)
            {
                _unitOfWork.Commit();
                output.Updated(l.Id);
            }
        }
    }
}