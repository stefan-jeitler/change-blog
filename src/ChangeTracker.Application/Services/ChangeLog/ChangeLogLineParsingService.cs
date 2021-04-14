using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.Services.ChangeLog
{
    public class ChangeLogLineParsingService
    {
        private readonly IChangeLogDao _changeLogDao;

        public ChangeLogLineParsingService(IChangeLogDao changeLogDao)
        {
            _changeLogDao = changeLogDao;
        }

        public async Task<Maybe<ChangeLogLine>> ParseAsync(IChangeLogLineParsingOutput output,
            ChangeLogLineParsingDto lineParsingDto)
        {
            if (!ChangeLogText.TryParse(lineParsingDto.Text, out var text))
            {
                output.InvalidChangeLogLineText(lineParsingDto.Text);
                return Maybe<ChangeLogLine>.None;
            }

            var labels = ExtractLabels(output, lineParsingDto.Labels, text);
            if (labels.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var issues = ExtractIssues(output, lineParsingDto.Issues, text);
            if (issues.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var nextFreePosition = await GetNexFreePositionAsync(output, lineParsingDto);
            if (nextFreePosition.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                lineParsingDto.VersionId, lineParsingDto.ProjectId,
                text, nextFreePosition.Value, DateTime.UtcNow,
                labels.Value, issues.Value);

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }

        private async Task<Maybe<uint>> GetNexFreePositionAsync(IChangeLogLineParsingOutput output,
            ChangeLogLineParsingDto lineParsingDto)
        {
            if (lineParsingDto.Position.HasValue)
            {
                return Maybe<uint>.From(lineParsingDto.Position.Value);
            }

            var changeLogInfo = await GetChangeLogsMetadataAsync(lineParsingDto);

            if (!changeLogInfo.IsPositionAvailable)
            {
                output.TooManyLines(ChangeLogsMetadata.MaxChangeLogLines);
                return Maybe<uint>.None;
            }

            return Maybe<uint>.From(changeLogInfo.NextFreePosition);
        }

        private async Task<ChangeLogsMetadata> GetChangeLogsMetadataAsync(ChangeLogLineParsingDto lineParsingDto)
        {
            var projectId = lineParsingDto.ProjectId;
            var versionId = lineParsingDto.VersionId;

            var changeLogInfo = versionId.HasValue
                ? await _changeLogDao.GetChangeLogsMetadataAsync(projectId, versionId.Value)
                : await _changeLogDao.GetPendingChangeLogsMetadataAsync(projectId);

            return changeLogInfo;
        }

        public static Maybe<List<Issue>> ExtractIssues(IChangeLogLineParsingOutput output, IEnumerable<string> issues,
            ChangeLogText text)
        {
            var parsedIssues = ParseIssues(issues);

            if (parsedIssues.IsFailure)
            {
                output.InvalidIssue(text, parsedIssues.Error);
                return Maybe<List<Issue>>.None;
            }

            if (parsedIssues.Value.Count > ChangeLogLine.MaxIssues)
            {
                output.TooManyIssues(text, ChangeLogLine.MaxIssues);
                return Maybe<List<Issue>>.None;
            }

            return Maybe<List<Issue>>.From(parsedIssues.Value);
        }

        private static Result<List<Issue>, string> ParseIssues(IEnumerable<string> issues)
        {
            var parsedIssues = new List<Issue>();

            foreach (var i in issues)
            {
                if (Issue.TryParse(i, out var issue))
                {
                    parsedIssues.Add(issue);
                }
                else
                {
                    return Result.Failure<List<Issue>, string>(i);
                }
            }

            return Result.Success<List<Issue>, string>(parsedIssues);
        }

        public static Maybe<List<Label>> ExtractLabels(IChangeLogLineParsingOutput output, IEnumerable<string> labels,
            ChangeLogText text)
        {
            var parsedLabels = ParseLabels(labels);

            if (parsedLabels.IsFailure)
            {
                output.InvalidLabel(text, parsedLabels.Error);
                return Maybe<List<Label>>.None;
            }

            if (parsedLabels.Value.Count > ChangeLogLine.MaxLabels)
            {
                output.TooManyLabels(text, ChangeLogLine.MaxLabels);
                return Maybe<List<Label>>.None;
            }

            return Maybe<List<Label>>.From(parsedLabels.Value);
        }

        private static Result<List<Label>, string> ParseLabels(IEnumerable<string> labels)
        {
            var parsedLabels = new List<Label>();
            foreach (var l in labels)
            {
                if (Label.TryParse(l, out var label))
                {
                    parsedLabels.Add(label);
                }
                else
                {
                    return Result.Failure<List<Label>, string>(l);
                }
            }

            return Result.Success<List<Label>, string>(parsedLabels);
        }
    }
}