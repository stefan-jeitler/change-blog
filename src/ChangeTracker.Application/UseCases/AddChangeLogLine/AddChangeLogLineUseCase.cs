using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.ChangeLog.Services;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public class AddChangeLogLineUseCase : IAddChangeLogLineUseCase
    {
        private readonly IProjectDao _projectDao;
        private readonly IVersionDao _versionDao;
        private readonly IChangeLogDao _changeLogDao;
        private readonly IUnitOfWork _unitOfWork;

        public AddChangeLogLineUseCase(IProjectDao projectDao, IVersionDao versionDao, IChangeLogDao changeLogDao, IUnitOfWork unitOfWork)
        {
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _changeLogDao = changeLogDao ?? throw new ArgumentNullException(nameof(changeLogDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(IAddChangeLogLineOutputPort output,
            AddChangeLogLineDto addChangeLogLineDto)
        {
            var labels = ExtractLabels(output, addChangeLogLineDto.Labels);
            if (labels.HasNoValue)
                return;

            var issues = ExtractIssues(output, addChangeLogLineDto.Issues);
            if (issues.HasNoValue)
                return;

            _unitOfWork.Start();

            var changeLogLine = await CreateChangeLogLineAsync(output, addChangeLogLineDto, labels.Value, issues.Value);
            if (changeLogLine.HasNoValue)
                return;

            await SaveChangeLogLine(output, changeLogLine.Value);
        }

        private static Maybe<List<Label>> ExtractLabels(IAddChangeLogLineOutputPort output, IEnumerable<string> labels)
        {
            var (parsedLabels, invalidLabels) = ParseLabels(labels);

            if (invalidLabels.Any())
            {
                output.InvalidLabels(invalidLabels);
                return Maybe<List<Label>>.None;
            }

            if (parsedLabels.Count > ChangeLogLine.MaxLabels)
            {
                output.TooManyLabels(ChangeLogLine.MaxLabels);
                return Maybe<List<Label>>.None;
            }

            return Maybe<List<Label>>.From(parsedLabels);
        }

        private static (List<Label> parsedLabels, List<string> invalidLabels) ParseLabels(IEnumerable<string> labels)
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

        private static Maybe<List<Issue>> ExtractIssues(IAddChangeLogLineOutputPort output, IEnumerable<string> issues)
        {
            var (parsedIssues, invalidIssues) = ParseIssues(issues);

            if (invalidIssues.Any())
            {
                output.InvalidIssues(invalidIssues);
                return Maybe<List<Issue>>.None;
            }

            if (parsedIssues.Count > ChangeLogLine.MaxIssues)
            {
                output.TooManyIssues(ChangeLogLine.MaxIssues);
                return Maybe<List<Issue>>.None;
            }

            return Maybe<List<Issue>>.From(parsedIssues);
        }

        private static (List<Issue> parsedIssues, List<string> invalidIssues) ParseIssues(IEnumerable<string> issues)
        {
            var parsedIssues = new List<Issue>();
            var invalidIssues = new List<string>();

            foreach (var i in issues)
            {
                if (Issue.TryParse(i, out var issue))
                {
                    parsedIssues.Add(issue);
                }
                else
                {
                    invalidIssues.Add(i);
                }
            }

            return (parsedIssues, invalidIssues);
        }

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddChangeLogLineOutputPort output,
            AddChangeLogLineDto addChangeLogLineDto, IEnumerable<Label> labels,
            IEnumerable<Issue> issues)
        {
            var project = await _projectDao.FindAsync(addChangeLogLineDto.ProjectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return Maybe<ChangeLogLine>.None;
            }

            var versionInfo = await GetVersionInfoAsync(output, addChangeLogLineDto, project.Value);
            if (versionInfo.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            if (!ChangeLogText.TryParse(addChangeLogLineDto.Text, out var changeLogLineText))
            {
                output.InvalidChangeLogLine(addChangeLogLineDto.Text);
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogInfo = await _changeLogDao.GetChangeLogInfoAsync(project.Value.Id, versionInfo.Value.Id);

            if (!changeLogInfo.IsPositionAvailable)
            {
                output.MaxChangeLogLinesReached(ChangeLogInfo.MaxChangeLogLines);
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                versionInfo.Value.Id,
                project.Value.Id,
                changeLogLineText,
                changeLogInfo.NextFreePosition,
                DateTime.UtcNow,
                labels,
                issues);

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }

        private async Task<Maybe<ClVersionInfo>> GetVersionInfoAsync(IAddChangeLogLineOutputPort output,
            AddChangeLogLineDto addChangeLogLineDto, Project project)
        {
            var noVersionInfo = Maybe<ClVersionInfo>.None;

            if (!ClVersion.TryParse(addChangeLogLineDto.Version, out var version))
            {
                output.InvalidVersionFormat();
                return noVersionInfo;
            }

            var versionInfo = await _versionDao.FindAsync(project.Id, version);
            if (versionInfo.HasNoValue)
            {
                output.VersionDoesNotExist();
                return noVersionInfo;
            }

            if (versionInfo.Value.IsReleased)
            {
                output.VersionAlreadyReleased(versionInfo.Value.ReleasedAt!.Value);
                return noVersionInfo;
            }

            if (versionInfo.Value.IsDeleted)
            {
                output.VersionDeleted(versionInfo.Value.DeletedAt!.Value);
                return noVersionInfo;
            }

            return versionInfo;
        }

        private async Task SaveChangeLogLine(IAddChangeLogLineOutputPort output, ChangeLogLine changeLogLine)
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