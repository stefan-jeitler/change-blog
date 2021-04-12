using System.Collections.Generic;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.Services.Issues
{
    public static class ExtractIssuesService
    {
        public static Maybe<List<Issue>> Extract(IExtractIssuesOutputPort output, IEnumerable<string> issues,
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
    }
}