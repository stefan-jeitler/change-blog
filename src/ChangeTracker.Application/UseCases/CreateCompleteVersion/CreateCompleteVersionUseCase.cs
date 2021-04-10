using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Application.Services.Issues;
using ChangeTracker.Application.Services.Labels;
using ChangeTracker.Application.UseCases.CreateCompleteVersion.DTOs;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.CreateCompleteVersion
{
    public class CreateCompleteVersionUseCase : ICreateCompleteVersionUseCase
    {
        private readonly IChangeLogDao _changeLogDao;
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public CreateCompleteVersionUseCase(IProjectDao projectDao, IVersionDao versionDao, IChangeLogDao changeLogDao,
            IUnitOfWork unitOfWork)
        {
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _changeLogDao = changeLogDao ?? throw new ArgumentNullException(nameof(changeLogDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(ICreateCompleteVersionOutputPort output, CompleteVersionDto versionDto)
        {
            var project = await _projectDao.FindAsync(versionDto.ProjectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return;
            }

            var newVersion = CreateNewVersion(output, 
                project.Value, versionDto.Version, 
                versionDto.ReleaseImmediately);

            if (newVersion.HasNoValue)
                return;

            var lines = CreateLines(output, versionDto.Lines, newVersion.Value);
            if (lines.HasNoValue)
                return;

            _unitOfWork.Start();
            await SaveNewReleasedVersion(output, newVersion.Value, lines.Value);
        }

        private static Maybe<IEnumerable<ChangeLogLine>> CreateLines(ICreateCompleteVersionOutputPort output,
            IEnumerable<ChangeLogLineDto> linesDto, ClVersionInfo newVersion)
        {
            var uniqueLines = linesDto
                .DistinctBy(x => x.Text)
                .ToList();

            if (uniqueLines.Count > ChangeLogInfo.MaxChangeLogLines)
            {
                output.MaxChangeLogLinesReached(ChangeLogInfo.MaxChangeLogLines);
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            var lines = uniqueLines
                .Select((lineDto, i) => CreateLine(output, lineDto, i, newVersion))
                .ToList();

            return lines.Any(x => x.HasNoValue)
                ? Maybe<IEnumerable<ChangeLogLine>>.None
                : Maybe<IEnumerable<ChangeLogLine>>.From(lines.Select(x => x.Value));
        }

        private static Maybe<ChangeLogLine> CreateLine(ICreateCompleteVersionOutputPort output, ChangeLogLineDto lineDto,
            int position, ClVersionInfo clVersionInfo)
        {
            var labels = ExtractLabelsService.Extract(output, lineDto.Labels);
            if (labels.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var issues = ExtractIssuesService.Extract(output, lineDto.Issues);
            if (issues.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            if (!ChangeLogText.TryParse(lineDto.Text, out var text))
            {
                output.InvalidChangeLogLineText(lineDto.Text);
                return Maybe<ChangeLogLine>.None;
            }

            var line = new ChangeLogLine(Guid.NewGuid(),
                clVersionInfo.Id, clVersionInfo.ProjectId, text,
                (uint) position, clVersionInfo.CreatedAt, labels.Value,
                issues.Value);

            return Maybe<ChangeLogLine>.From(line);
        }

        private static Maybe<ClVersionInfo> CreateNewVersion(ICreateCompleteVersionOutputPort output,
            Project project, string version, bool releaseImmediately)
        {
            if (!ClVersion.TryParse(version, out var clVersion))
            {
                output.InvalidVersionFormat(version);
                return Maybe<ClVersionInfo>.None;
            }

            if (!clVersion.Match(project.VersioningScheme))
            {
                output.VersionDoesNotMatchScheme(version);
                return Maybe<ClVersionInfo>.None;
            }

            var utcNow = DateTime.UtcNow;
            DateTime? releaseDate = releaseImmediately ? utcNow : null;
            var versionInfo = new ClVersionInfo(Guid.NewGuid(),
                project.Id,
                clVersion,
                releaseDate,
                utcNow,
                null);

            return Maybe<ClVersionInfo>.From(versionInfo);
        }

        private async Task SaveNewReleasedVersion(ICreateCompleteVersionOutputPort output, ClVersionInfo newVersion,
            IEnumerable<ChangeLogLine> lines)
        {
            await _versionDao
                .AddAsync(newVersion)
                .Bind(x => _changeLogDao.AddLinesAsync(lines))
                .Match(Finish, c => output.Conflict(c));

            void Finish(int count)
            {
                output.Created(newVersion.Id);
                _unitOfWork.Commit();
            }
        }
    }
}