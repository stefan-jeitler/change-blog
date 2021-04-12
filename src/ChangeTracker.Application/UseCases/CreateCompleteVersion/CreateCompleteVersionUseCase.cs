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
// ReSharper disable UseDeconstructionOnParameter

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
            IEnumerable<ChangeLogLineDto> linesDto, ClVersion newVersion)
        {
            var uniqueLines = linesDto
                .DistinctBy(x => x.Text)
                .ToList();

            if (uniqueLines.Count > ChangeLogsMetadata.MaxChangeLogLines)
            {
                output.MaxChangeLogLinesReached(ChangeLogsMetadata.MaxChangeLogLines);
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            var lines = new List<ChangeLogLine>();
            foreach (var (lineDto, i) in uniqueLines.Select((x, i) => (x, i)))
            {
                var line = CreateLine(output, lineDto, i, newVersion);

                if (line.HasNoValue)
                {
                    return Maybe<IEnumerable<ChangeLogLine>>.None;
                }

                lines.Add(line.Value);
            }

            return Maybe<IEnumerable<ChangeLogLine>>.From(lines);
        }

        private static Maybe<ChangeLogLine> CreateLine(ICreateCompleteVersionOutputPort output, ChangeLogLineDto lineDto,
            int position, ClVersion clVersion)
        {
            if (!ChangeLogText.TryParse(lineDto.Text, out var text))
            {
                output.InvalidChangeLogLineText(lineDto.Text);
                return Maybe<ChangeLogLine>.None;
            }

            var labels = ExtractLabelsService.Extract(output, lineDto.Labels, text);
            if (labels.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var issues = ExtractIssuesService.Extract(output, lineDto.Issues, text);
            if (issues.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var line = new ChangeLogLine(Guid.NewGuid(),
                clVersion.Id, clVersion.ProjectId, text,
                (uint) position, clVersion.CreatedAt, labels.Value,
                issues.Value);

            return Maybe<ChangeLogLine>.From(line);
        }

        private static Maybe<ClVersion> CreateNewVersion(ICreateCompleteVersionOutputPort output,
            Project project, string version, bool releaseImmediately)
        {
            if (!ClVersionValue.TryParse(version, out var clVersion))
            {
                output.InvalidVersionFormat(version);
                return Maybe<ClVersion>.None;
            }

            if (!clVersion.Match(project.VersioningScheme))
            {
                output.VersionDoesNotMatchScheme(version);
                return Maybe<ClVersion>.None;
            }

            var utcNow = DateTime.UtcNow;
            DateTime? releaseDate = releaseImmediately ? utcNow : null;
            var versionInfo = new ClVersion(Guid.NewGuid(),
                project.Id,
                clVersion,
                releaseDate,
                utcNow,
                null);

            return Maybe<ClVersion>.From(versionInfo);
        }

        private async Task SaveNewReleasedVersion(ICreateCompleteVersionOutputPort output, ClVersion newVersion,
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