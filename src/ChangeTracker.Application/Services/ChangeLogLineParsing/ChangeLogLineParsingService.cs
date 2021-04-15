using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.Services.ChangeLogLineParsing
{
    public class ChangeLogLineParsingService
    {
        private readonly IChangeLogDao _changeLogDao;

        public ChangeLogLineParsingService(IChangeLogDao changeLogDao)
        {
            _changeLogDao = changeLogDao;
        }

        public async Task<Maybe<ChangeLogLine>> ParseAsync(IChangeLogLineParsingOutput output,
            LineParsingRequestModel lineParsingRequestModel)
        {
            if (!ChangeLogText.TryParse(lineParsingRequestModel.Text, out var text))
            {
                output.InvalidChangeLogLineText(lineParsingRequestModel.Text);
                return Maybe<ChangeLogLine>.None;
            }

            var labels = ExtractLabels(output, lineParsingRequestModel.Labels, text);
            if (labels.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var issues = ExtractIssues(output, lineParsingRequestModel.Issues, text);
            if (issues.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var nextFreePosition = await GetNexFreePositionAsync(output, lineParsingRequestModel);
            if (nextFreePosition.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                lineParsingRequestModel.VersionId, lineParsingRequestModel.ProjectId,
                text, nextFreePosition.Value, DateTime.UtcNow,
                labels.Value, issues.Value);

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }

        private async Task<Maybe<uint>> GetNexFreePositionAsync(IChangeLogLineParsingOutput output,
            LineParsingRequestModel lineParsingRequestModel)
        {
            if (lineParsingRequestModel.Position.HasValue)
            {
                return Maybe<uint>.From(lineParsingRequestModel.Position.Value);
            }

            var changeLogsMetadata = await GetChangeLogsMetadataAsync(lineParsingRequestModel);

            if (!changeLogsMetadata.IsPositionAvailable)
            {
                output.TooManyLines(ChangeLogsMetadata.MaxChangeLogLines);
                return Maybe<uint>.None;
            }

            return Maybe<uint>.From(changeLogsMetadata.NextFreePosition);
        }

        private async Task<ChangeLogsMetadata> GetChangeLogsMetadataAsync(LineParsingRequestModel lineParsingRequestModel)
        {
            var projectId = lineParsingRequestModel.ProjectId;
            var versionId = lineParsingRequestModel.VersionId;

            var changeLogsMetadata = versionId.HasValue
                ? await _changeLogDao.GetChangeLogsMetadataAsync(projectId, versionId.Value)
                : await _changeLogDao.GetPendingChangeLogsMetadataAsync(projectId);

            return changeLogsMetadata;
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