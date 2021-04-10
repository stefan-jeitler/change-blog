using System.Collections.Generic;
using System.Linq;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.Services.Issues
{
    public static class ExtractIssuesService
    {
        public static Maybe<List<Issue>> Extract(IExtractIssuesOutputPort output, IEnumerable<string> issues)
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
    }
}