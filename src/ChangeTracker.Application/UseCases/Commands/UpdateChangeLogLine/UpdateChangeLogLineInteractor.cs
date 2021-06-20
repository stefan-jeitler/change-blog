using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLog;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using ChangeTracker.Application.UseCases.Commands.DeleteChangeLogLine;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Commands.UpdateChangeLogLine
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

        public async Task ExecuteAsync(IUpdateChangeLogLineOutputPort output,
            UpdateChangeLogLineRequestModel requestModel)
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

            switch (requestModel.ChangeLogLineType)
            {
                case ChangeLogLineType.Pending when !existingLine.Value.IsPending:
                    output.RequestedLineIsNotPending(existingLine.Value.Id);
                    return;
                case ChangeLogLineType.NotPending when existingLine.Value.IsPending:
                    output.RequestedLineIsPending(existingLine.Value.Id);
                    return;
            }

            var changeLogs = await _changeLogQueries.GetChangeLogsAsync(existingLine.Value.ProductId,
                existingLine.Value.VersionId);

            if (requestModel.Text is not null &&
                changeLogs.ContainsText(parsedNewLine.Value.Text))
            {
                output.LineWithSameTextAlreadyExists(requestModel.Text);
                return;
            }

            var updatedLine = CreateUpdatedLine(requestModel, existingLine, parsedNewLine);
            await UpdateLineAsync(output, updatedLine);
        }

        private static ChangeLogLine CreateUpdatedLine(UpdateChangeLogLineRequestModel requestModel,
            Maybe<ChangeLogLine> existingLine, Maybe<LineParserResponseModel> parsedNewLine)
        {
            var existing = existingLine.Value;

            var newText = requestModel.Text is null
                ? existing.Text
                : parsedNewLine.Value.Text;

            var newLabels = requestModel.Labels is null
                ? existing.Labels
                : parsedNewLine.Value.Labels;

            var newIssues = requestModel.Issues is null
                ? existing.Issues
                : parsedNewLine.Value.Issues;

            return new ChangeLogLine(existing.Id,
                existing.VersionId,
                existing.ProductId,
                newText,
                existing.Position,
                existing.CreatedAt,
                newLabels,
                newIssues,
                existing.CreatedByUser,
                existing.DeletedAt);
        }

        private static Maybe<LineParserResponseModel> ParseLine(ILineParserOutput output,
            UpdateChangeLogLineRequestModel requestModel)
        {
            var lineParsingRequestModel =
                new LineParserRequestModel(
                    requestModel.Text ?? "Fake Line",
                    requestModel.Labels ?? new List<string>(0),
                    requestModel.Issues ?? new List<string>(0));

            return LineParser.Parse(output, lineParsingRequestModel);
        }

        private async Task UpdateLineAsync(IUpdateChangeLogLineOutputPort output, ChangeLogLine updatedLine)
        {
            await _changeLogCommands.UpdateLineAsync(updatedLine)
                .Match(Finish, output.Conflict);

            void Finish(ChangeLogLine l)
            {
                _unitOfWork.Commit();
                output.Updated(l.Id);
            }
        }
    }
}