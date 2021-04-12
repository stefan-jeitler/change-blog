using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Services.Issues;
using ChangeTracker.Application.Services.Labels;
using ChangeTracker.Application.Services.NotReleasedVersion;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public class AddChangeLogLineUseCase : IAddChangeLogLineUseCase
    {
        private readonly IChangeLogDao _changeLogDao;

        private readonly NotReleasedVersionService _notReleasedVersion;
        private readonly IUnitOfWork _unitOfWork;

        public AddChangeLogLineUseCase(IChangeLogDao changeLogDao,
            IUnitOfWork unitOfWork, NotReleasedVersionService notReleasedVersionService)
        {
            _changeLogDao = changeLogDao ?? throw new ArgumentNullException(nameof(changeLogDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _notReleasedVersion = notReleasedVersionService ??
                                  throw new ArgumentNullException(nameof(notReleasedVersionService));
        }

        public async Task ExecuteAsync(IAddChangeLogLineOutputPort output,
            ChangeLogLineDto changeLogLineDto)
        {
            if (!ClVersionValue.TryParse(changeLogLineDto.Version, out var version))
            {
                output.InvalidVersionFormat();
                return;
            }

            if (!ChangeLogText.TryParse(changeLogLineDto.Text, out var text))
            {
                output.InvalidChangeLogLineText(changeLogLineDto.Text);
                return;
            }

            var labels = ExtractLabelsService.Extract(output, changeLogLineDto.Labels, text);
            if (labels.HasNoValue)
                return;

            var issues = ExtractIssuesService.Extract(output, changeLogLineDto.Issues, text);
            if (issues.HasNoValue)
                return;

            _unitOfWork.Start();

            var versionInfo = await _notReleasedVersion.FindAsync(output, changeLogLineDto.ProjectId, version);
            var newChangeLogLine = await versionInfo
                .Bind(x => CreateChangeLogLineAsync(output, x, text, labels.Value, issues.Value));

            if (newChangeLogLine.HasNoValue)
                return;

            await SaveChangeLogLineAsync(output, newChangeLogLine.Value);
        }

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddChangeLogLineOutputPort output,
            ClVersion version, ChangeLogText text, IEnumerable<Label> labels, IEnumerable<Issue> issues)
        {
            var changeLogInfo = await _changeLogDao.GetChangeLogsMetadataAsync(version.ProjectId, version.Id);
            if (!changeLogInfo.IsPositionAvailable)
            {
                output.MaxChangeLogLinesReached(ChangeLogsMetadata.MaxChangeLogLines);
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                version.Id,
                version.ProjectId,
                text,
                changeLogInfo.NextFreePosition,
                DateTime.UtcNow,
                labels,
                issues);

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }

        private async Task SaveChangeLogLineAsync(IAddChangeLogLineOutputPort output, ChangeLogLine changeLogLine)
        {
            await _changeLogDao
                .AddLineAsync(changeLogLine)
                .Match(Finish, c => output.Conflict(c));

            void Finish(ChangeLogLine l)
            {
                output.Created(l.Id);
                _unitOfWork.Commit();
            }
        }
    }
}