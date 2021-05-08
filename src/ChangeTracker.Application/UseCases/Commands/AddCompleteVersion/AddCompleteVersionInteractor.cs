﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using ChangeTracker.Application.UseCases.Commands.AddCompleteVersion.Models;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

// ReSharper disable UseDeconstructionOnParameter

namespace ChangeTracker.Application.UseCases.Commands.AddCompleteVersion
{
    public class AddCompleteVersionInteractor : IAddCompleteVersion
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AddCompleteVersionInteractor(IProjectDao projectDao, IVersionDao versionDao,
            IUnitOfWork unitOfWork, IChangeLogCommandsDao changeLogCommands)
        {
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        }

        public async Task ExecuteAsync(IAddCompleteVersionOutputPort output,
            CompleteVersionRequestModel versionRequestModel)
        {
            var project = await _projectDao.FindProjectAsync(versionRequestModel.ProjectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return;
            }

            var newVersion = CreateNewVersion(output, project.Value, versionRequestModel.Version,
                versionRequestModel.ReleaseImmediately);
            if (newVersion.HasNoValue)
                return;

            if ((await _versionDao.FindVersionAsync(project.Value.Id, newVersion.Value.Value)).HasValue)
            {
                output.VersionAlreadyExists(newVersion.Value.Value);
                return;
            }

            var lines = newVersion.Bind(v => CreateLines(output, versionRequestModel.Lines, v));
            if (lines.HasNoValue)
                return;

            _unitOfWork.Start();
            await SaveCompleteVersionAsync(output, newVersion.Value, lines.Value);
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

        private static Maybe<IEnumerable<ChangeLogLine>> CreateLines(IAddCompleteVersionOutputPort output,
            IEnumerable<ChangeLogLineRequestModel> requestModel, ClVersion newVersion)
        {
            var lineCandidates = requestModel
                .ToList();

            var duplicates = lineCandidates
                .GroupBy(x => x.Text)
                .Where(x => x.Skip(1).Any())
                .Select(x => x.Key)
                .ToList();

            if (duplicates.Any())
            {
                output.LinesWithSameTextsAreNotAllowed(duplicates);
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            if (lineCandidates.Count > ChangeLogs.MaxLines)
            {
                output.TooManyLines(ChangeLogs.MaxLines);
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            return ParseLines(output, lineCandidates, newVersion);
        }

        private static Maybe<IEnumerable<ChangeLogLine>> ParseLines(ILineParserOutput output,
            IEnumerable<ChangeLogLineRequestModel> lineCandidates, ClVersion newVersion)
        {
            var lines = new List<ChangeLogLine>();
            foreach (var (lineRequestModel, i) in lineCandidates.Select((x, i) => (x, i)))
            {
                var line = CreateLine(output, lineRequestModel, newVersion, (uint) i);

                if (line.HasNoValue)
                {
                    return Maybe<IEnumerable<ChangeLogLine>>.None;
                }

                lines.Add(line.Value);
            }

            return Maybe<IEnumerable<ChangeLogLine>>.From(lines);
        }

        private static Maybe<ChangeLogLine> CreateLine(ILineParserOutput output,
            ChangeLogLineRequestModel requestModel, ClVersion clVersion, uint position)
        {
            var lineParsingRequestModel =
                new LineParserRequestModel(requestModel.Text, requestModel.Labels, requestModel.Issues);

            var parsedLine = LineParser.Parse(output, lineParsingRequestModel);
            if (parsedLine.HasNoValue)
            {
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                clVersion.Id, clVersion.ProjectId,
                parsedLine.Value.Text, position, DateTime.UtcNow,
                parsedLine.Value.Labels, parsedLine.Value.Issues);

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }

        private async Task SaveCompleteVersionAsync(IAddCompleteVersionOutputPort output, ClVersion newVersion,
            IEnumerable<ChangeLogLine> lines)
        {
            await _versionDao
                .AddVersionAsync(newVersion)
                .Bind(_ => _changeLogCommands.AddLinesAsync(lines))
                .Match(Finish, c => output.Conflict(c));

            void Finish(int count)
            {
                _unitOfWork.Commit();
                output.Created(newVersion.Id);
            }
        }
    }
}