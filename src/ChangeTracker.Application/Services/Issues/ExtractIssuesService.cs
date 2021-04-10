using System.Collections.Generic;
using System.Linq;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.Services.Issues
{
    public class ExtractIssuesService
    {
        private readonly IExtractIssuesOutputPort _output;

        public ExtractIssuesService(IExtractIssuesOutputPort output)
        {
            _output = output;
        }

        public Maybe<List<Issue>> Extract(IEnumerable<string> issues)
        {
            var (parsedIssues, invalidIssues) = ParseIssues(issues);

            if (invalidIssues.Any())
            {
                _output.InvalidIssues(invalidIssues);
                return Maybe<List<Issue>>.None;
            }

            if (parsedIssues.Count > ChangeLogLine.MaxIssues)
            {
                _output.TooManyIssues(ChangeLogLine.MaxIssues);
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