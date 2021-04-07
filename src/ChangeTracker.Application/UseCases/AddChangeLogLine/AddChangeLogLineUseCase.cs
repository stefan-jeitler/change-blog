using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.ChangeLog.Services;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public class AddChangeLogLineUseCase : IAddChangeLogLineUseCase
    {
        private readonly IChangeLogDao _changeLogDao;
        private readonly IProjectDao _projectDao;
        private readonly IVersionDao _versionDao;

        public AddChangeLogLineUseCase(IProjectDao projectDao, IVersionDao versionDao, IChangeLogDao changeLogDao)
        {
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _changeLogDao = changeLogDao ?? throw new ArgumentNullException(nameof(changeLogDao));
        }

        public async Task ExecuteAsync(IAddChangeLogLineOutputPort output,
            AddChangeLogLineToVersionDto addChangeLogLineToVersionDto)
        {
            var (labels, invalidLabels) = ParseLabels(addChangeLogLineToVersionDto.Labels);
            if (invalidLabels.Any())
            {
                output.InvalidLabels(invalidLabels);
                return;
            }

            if (labels.Count > ChangeLogLine.MaxLabels)
            {
                output.TooManyLabels(ChangeLogLine.MaxLabels);
                return;
            }

            var changeLogLine = await CreateChangeLogLineAsync(output, addChangeLogLineToVersionDto, labels);
            if (changeLogLine.HasNoValue)
            {
                return;
            }

            await SaveChangeLogLine(output, changeLogLine.Value);
        }

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddChangeLogLineOutputPort output,
            AddChangeLogLineToVersionDto addChangeLogLineToVersionDto, IEnumerable<Label> labels)
        {
            var project = await _projectDao.FindAsync(addChangeLogLineToVersionDto.ProjectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return Maybe<ChangeLogLine>.None;
            }

            if (!ClVersion.TryParse(addChangeLogLineToVersionDto.Version, out var version))
            {
                output.InvalidVersionFormat();
                return Maybe<ChangeLogLine>.None;
            }

            var versionInfo = await _versionDao.FindAsync(project.Value.Id, version);
            if (versionInfo.HasNoValue)
            {
                output.VersionDoesNotExist();
                return Maybe<ChangeLogLine>.None;
            }

            if (!ChangeLogText.TryParse(addChangeLogLineToVersionDto.Text, out var changeLogLineText))
            {
                output.InvalidChangeLogLine(addChangeLogLineToVersionDto.Text);
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogInfo = await _changeLogDao.GetChangeLogInfoAsync(project.Value.Id, versionInfo.Value.Id);
            var changeLogLineVersionService = new VersionChangeLogService(changeLogInfo);

            if (!changeLogLineVersionService.IsPositionAvailable)
            {
                output.MaxChangeLogReached(VersionChangeLogService.MaxChangeLog);
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                versionInfo.Value.Id,
                project.Value.Id,
                changeLogLineText,
                changeLogLineVersionService.NextFreePosition,
                DateTime.UtcNow,
                labels, 
                Array.Empty<Issue>());

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }

        private async Task SaveChangeLogLine(IAddChangeLogLineOutputPort output, ChangeLogLine changeLogLine)
        {
            var result = await _changeLogDao.AddChangeLogLineAsync(changeLogLine);

            result.Switch(
                r => output.Created(r.Id),
                c => output.Conflict(c.Reason));
        }

        private static (List<Label> labels, List<string> invalidLabels) ParseLabels(IEnumerable<string> labels)
        {
            var parsedLabels = new List<Label>();
            var invalidLabels = new List<string>();
            foreach (var l in labels)
            {
                if (Label.TryParse(l, out var label))
                {
                    parsedLabels.Add(label);
                }
                else
                {
                    invalidLabels.Add(l);
                }
            }

            return (parsedLabels, invalidLabels);
        }
    }
}