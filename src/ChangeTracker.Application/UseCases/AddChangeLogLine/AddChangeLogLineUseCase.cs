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
        private readonly IUnitOfWork _unitOfWork;

        private readonly NotReleasedVersionService _notReleasedVersion;

        public AddChangeLogLineUseCase(IChangeLogDao changeLogDao,
            IUnitOfWork unitOfWork, NotReleasedVersionService notReleasedVersionService)
        {
            _changeLogDao = changeLogDao ?? throw new ArgumentNullException(nameof(changeLogDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _notReleasedVersion = notReleasedVersionService ??
                                  throw new ArgumentNullException(nameof(notReleasedVersionService));
        }

        public async Task ExecuteAsync(IAddChangeLogLineOutputPort output,
            AddChangeLogLineDto changeLogLineDto)
        {
            if (!ClVersion.TryParse(changeLogLineDto.Version, out var version))
            {
                output.InvalidVersionFormat();
                return;
            }

            if (!ChangeLogText.TryParse(changeLogLineDto.Text, out var text))
            {
                output.InvalidChangeLogLineText(changeLogLineDto.Text);
                return;
            }

            var extractLabelsService = new ExtractLabelsService(output);
            var labels = extractLabelsService.Extract(changeLogLineDto.Labels);
            if (labels.HasNoValue)
                return;

            var extractIssuesService = new ExtractIssuesService(output);
            var issues = extractIssuesService.Extract(changeLogLineDto.Issues);
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
            ClVersionInfo versionInfo, ChangeLogText text, IEnumerable<Label> labels, IEnumerable<Issue> issues)
        {
            var changeLogInfo = await _changeLogDao.GetChangeLogInfoAsync(versionInfo.ProjectId, versionInfo.Id);
            if (!changeLogInfo.IsPositionAvailable)
            {
                output.MaxChangeLogLinesReached(ChangeLogInfo.MaxChangeLogLines);
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                versionInfo.Id,
                versionInfo.ProjectId,
                text,
                (uint)changeLogInfo.NextFreePosition,
                DateTime.UtcNow,
                labels,
                issues);

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }

        private async Task SaveChangeLogLineAsync(IAddChangeLogLineOutputPort output, ChangeLogLine changeLogLine)
        {
            var result = await _changeLogDao.AddChangeLogLineAsync(changeLogLine);

            result.Switch(
                r =>
                {
                    output.Created(r.Id);
                    _unitOfWork.Commit();
                },
                c => output.Conflict(c.Reason));
        }
    }
}