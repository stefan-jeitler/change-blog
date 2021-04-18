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
        private readonly IChangeLogDao _changeLogDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public UpdateChangeLogLineInteractor(IChangeLogDao changeLogDao, IUnitOfWork unitOfWork, IVersionDao versionDao)
        {
            _changeLogDao = changeLogDao;
            _unitOfWork = unitOfWork;
            _versionDao = versionDao;
        }

        public async Task ExecuteAsync(IUpdateLineOutputPort output, ChangeLogLineRequestModel requestModel)
        {
            _unitOfWork.Start();

            var existingLine = await _changeLogDao.FindLineAsync(requestModel.ChangeLogLineId);
            if (existingLine.HasNoValue)
            {
                output.ChangeLogLineDoesNotExist();
                return;
            }

            if (!await CanUpdateLineAsync(output, existingLine.Value))
                return;

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

        private async Task<bool> CanUpdateLineAsync(IUpdateLineOutputPort output, ChangeLogLine existingLine)
        {
            if (existingLine.IsPending)
                return true;

            var version = await _versionDao.GetVersionAsync(existingLine.ProjectId, existingLine.VersionId!.Value);

            if (version.IsReleased)
            {
                output.RelatedVersionAlreadyReleased();
                return false;
            }

            if (version.IsDeleted)
            {
                output.RelatedVersionDeleted();
                return false;
            }

            return true;
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

            await _changeLogDao.UpdateLineAsync(line)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(int c)
            {
                _unitOfWork.Commit();
                output.Updated(line.Id);
            }
        }
    }
}