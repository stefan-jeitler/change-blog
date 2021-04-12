﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Services.Issues;
using ChangeTracker.Application.Services.Labels;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public class AddPendingChangeLogLineUseCase : IAddPendingChangeLogLineUseCase
    {
        private readonly IChangeLogDao _changeLogDao;
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;

        public AddPendingChangeLogLineUseCase(IProjectDao projectDao, IChangeLogDao changeLogDao,
            IUnitOfWork unitOfWork)
        {
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _changeLogDao = changeLogDao ?? throw new ArgumentNullException(nameof(changeLogDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(IAddPendingChangeLogLineOutputPort outputPort,
            PendingChangeLogLineDto changeLogLineDto)
        {
            if (!ChangeLogText.TryParse(changeLogLineDto.Text, out var text))
            {
                outputPort.InvalidChangeLogLineText(changeLogLineDto.Text);
                return;
            }

            var labels = ExtractLabelsService.Extract(outputPort, changeLogLineDto.Labels, text);
            if (labels.HasNoValue)
                return;

            var issues = ExtractIssuesService.Extract(outputPort, changeLogLineDto.Issues, text);
            if (issues.HasNoValue)
                return;

            _unitOfWork.Start();

            var line = await CreateChangeLogLineAsync(outputPort, changeLogLineDto, text, labels.Value, issues.Value);
            if (line.HasNoValue)
                return;

            await SaveChangeLogLineAsync(outputPort, line.Value);
        }


        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddPendingChangeLogLineOutputPort outputPort,
            PendingChangeLogLineDto changeLogLineDto, ChangeLogText text, IEnumerable<Label> labels,
            IEnumerable<Issue> issues)
        {
            var project = await _projectDao.FindAsync(changeLogLineDto.ProjectId);
            if (project.HasNoValue)
            {
                outputPort.ProjectDoesNotExist();
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogInfo = await _changeLogDao.GetPendingChangeLogMetadataAsync(project.Value.Id);
            if (!changeLogInfo.IsPositionAvailable)
            {
                outputPort.MaxChangeLogLinesReached(ChangeLogsMetadata.MaxChangeLogLines);
                return Maybe<ChangeLogLine>.None;
            }

            var line = new ChangeLogLine(Guid.NewGuid(),
                null,
                project.Value.Id,
                text,
                changeLogInfo.NextFreePosition,
                DateTime.UtcNow, labels, issues);

            return Maybe<ChangeLogLine>.From(line);
        }


        private async Task SaveChangeLogLineAsync(IAddPendingChangeLogLineOutputPort outputPort, ChangeLogLine line)
        {
            await _changeLogDao
                .AddLineAsync(line)
                .Match(Finish, c => outputPort.Conflict(c));

            void Finish(ChangeLogLine l)
            {
                outputPort.Created(l.Id);
                _unitOfWork.Commit();
            }
        }
    }
}