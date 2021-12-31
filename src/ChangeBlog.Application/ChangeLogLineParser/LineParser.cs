using System.Collections.Generic;
using System.Collections.Immutable;
using ChangeBlog.Domain.ChangeLog;
using CSharpFunctionalExtensions;

// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable InvertIf

namespace ChangeBlog.Application.ChangeLogLineParser;

public static class LineParser
{
    public static Maybe<LineParserResponseModel> Parse(ILineParserOutput output,
        LineParserRequestModel lineParserRequestModel)
    {
        if (!ChangeLogText.TryParse(lineParserRequestModel.Text, out var text))
        {
            output.InvalidChangeLogLineText(lineParserRequestModel.Text);
            return Maybe<LineParserResponseModel>.None;
        }

        var labels = ExtractLabels(output, lineParserRequestModel.Labels, text);
        if (labels.HasNoValue)
        {
            return Maybe<LineParserResponseModel>.None;
        }

        var issues = ExtractIssues(output, lineParserRequestModel.Issues, text);
        if (issues.HasNoValue)
        {
            return Maybe<LineParserResponseModel>.None;
        }

        return Maybe<LineParserResponseModel>.From(new LineParserResponseModel(text,
            labels.GetValueOrThrow().ToImmutableHashSet(),
            issues.GetValueOrThrow().ToImmutableHashSet()));
    }

    private static Maybe<List<Issue>> ExtractIssues(ILineParserOutput output, IEnumerable<string> issues,
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

    private static Maybe<List<Label>> ExtractLabels(ILineParserOutput output, IEnumerable<string> labels,
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