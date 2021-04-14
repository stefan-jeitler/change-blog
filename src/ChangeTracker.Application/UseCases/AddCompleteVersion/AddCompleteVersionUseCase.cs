using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Application.Services.ChangeLog;
using ChangeTracker.Application.UseCases.AddCompleteVersion.DTOs;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable UseDeconstructionOnParameter

namespace ChangeTracker.Application.UseCases.AddCompleteVersion
{
    public class AddCompleteVersionUseCase : IAddCompleteVersionUseCase
    {
        private readonly IChangeLogDao _changeLogDao;
        private readonly ChangeLogLineParsingService _changeLogLineParsing;
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AddCompleteVersionUseCase(IProjectDao projectDao, IVersionDao versionDao, IChangeLogDao changeLogDao,
            IUnitOfWork unitOfWork, ChangeLogLineParsingService changeLogLineParsing)
        {
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _changeLogDao = changeLogDao ?? throw new ArgumentNullException(nameof(changeLogDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogLineParsing = changeLogLineParsing;
        }

        public async Task ExecuteAsync(IAddCompleteVersionOutputPort output, CompleteVersionDto versionDto)
        {
            var project = await _projectDao.FindAsync(versionDto.ProjectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return;
            }

            var newVersion = CreateNewVersion(output, project.Value, versionDto.Version, versionDto.ReleaseImmediately);
            if (newVersion.HasNoValue)
                return;

            if ((await _versionDao.FindAsync(project.Value.Id, newVersion.Value.Value)).HasValue)
            {
                output.VersionAlreadyExists(newVersion.Value.Value);
                return;
            }

            var lines = await newVersion.Bind(v => CreateLines(output, versionDto.Lines, v));
            if (lines.HasNoValue)
                return;

            _unitOfWork.Start();
            await SaveCompleteVersionAsync(output, newVersion.Value, lines.Value);
        }

        private async Task<Maybe<IEnumerable<ChangeLogLine>>> CreateLines(IChangeLogLineParsingOutput output,
            IEnumerable<ChangeLogLineDto> linesDto, ClVersion newVersion)
        {
            var uniqueLines = linesDto
                .DistinctBy(x => x.Text)
                .ToList();

            if (uniqueLines.Count > ChangeLogsMetadata.MaxChangeLogLines)
            {
                output.TooManyLines(ChangeLogsMetadata.MaxChangeLogLines);
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            var lines = new List<ChangeLogLine>();
            foreach (var (lineDto, i) in uniqueLines.Select((x, i) => (x, i)))
            {
                var line = await CreateLineAsync(output, lineDto, newVersion, (uint) i);

                if (line.HasNoValue)
                {
                    return Maybe<IEnumerable<ChangeLogLine>>.None;
                }

                lines.Add(line.Value);
            }

            return Maybe<IEnumerable<ChangeLogLine>>.From(lines);
        }

        private async Task<Maybe<ChangeLogLine>> CreateLineAsync(IChangeLogLineParsingOutput output,
            ChangeLogLineDto lineDto, ClVersion clVersion, uint position)
        {
            var parsingDto = new ChangeLogLineParsingDto(clVersion.ProjectId,
                clVersion.Id,
                lineDto.Text,
                lineDto.Labels,
                lineDto.Issues,
                position);

            return await _changeLogLineParsing.ParseAsync(output, parsingDto);
        }

        private static Maybe<ClVersion> CreateNewVersion(IAddCompleteVersionOutputPort output,
            Project project, string versionCandidate, bool releaseImmediately)
        {
            if (!ClVersionValue.TryParse(versionCandidate, out var clVersion))
            {
                output.InvalidVersionFormat(versionCandidate);
                return Maybe<ClVersion>.None;
            }

            if (!clVersion.Match(project.VersioningScheme))
            {
                output.VersionDoesNotMatchScheme(clVersion.Value);
                return Maybe<ClVersion>.None;
            }

            var utcNow = DateTime.UtcNow;
            DateTime? releaseDate = releaseImmediately ? utcNow : null;
            var version = new ClVersion(Guid.NewGuid(),
                project.Id,
                clVersion,
                releaseDate,
                utcNow,
                null);

            return Maybe<ClVersion>.From(version);
        }

        private async Task SaveCompleteVersionAsync(IAddCompleteVersionOutputPort output, ClVersion newVersion,
            IEnumerable<ChangeLogLine> lines)
        {
            await _versionDao
                .AddAsync(newVersion)
                .Bind(_ => _changeLogDao.AddLinesAsync(lines))
                .Match(Finish, c => output.Conflict(c));

            void Finish(int count)
            {
                output.Created(newVersion.Id);
                _unitOfWork.Commit();
            }
        }
    }
}