using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
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
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries)); ;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork)); ;
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands)); ;
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

            if (NothingChanged(existingLine.Value, parsedNewLine.Value))
            {
                output.NotModified();
                return;
            }

            await UpdateLineAsync(output, existingLine.Value, parsedNewLine.Value);
        }

        private static bool NothingChanged(ChangeLogLine existingLine, LineParserResponseModel parsedNewLine)
        {
            return existingLine.Text == parsedNewLine.Text &&
                   Same(existingLine.Labels, parsedNewLine.Labels) &&
                   Same(existingLine.Issues, parsedNewLine.Issues);

            static bool Same<T>(IImmutableSet<T> existingItems, IReadOnlyCollection<T> newItems)
                => existingItems.Count == newItems.Count
                   && newItems.All(existingItems.Contains);
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

            void Finish(int c)
            {
                _unitOfWork.Commit();
                output.Updated(line.Id);
            }
        }
    }
}