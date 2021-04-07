using System;
using System.Collections.Generic;
using System.Linq;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ChangeLogTests
{
    public class ChangeLogLineIssuesTests
    {
        private static readonly Issue TestIssue = Issue.Parse("#1234");
        private const int TestPosition = 5;
        private static readonly Guid TestId = Guid.Parse("51d89265-52c2-4a38-a0fe-b99bdc5523d0");
        private static readonly Guid TestVersionId = Guid.Parse("66845d0a-45bc-4834-96d0-b48c2c403628");
        private static readonly Guid TestProjectId = Guid.Parse("ef5656e5-15f0-418d-b3a4-b69f1c3abac5");
        private static readonly ChangeLogText TestText = ChangeLogText.Parse("New feature added");
        private static readonly DateTime TestCreationDate = DateTime.Parse("2021-04.02T18:28");
        private static readonly DateTime TestDeletionDate = DateTime.Parse("2021-04.02T18:28");

        [Fact]
        public void Create_WithIssues_IssuesExists()
        {
            var line = new ChangeLogLine(TestId,
                TestVersionId,
                TestProjectId,
                TestText,
                TestPosition,
                TestCreationDate,
                Array.Empty<Label>(),
                new List<Issue> {TestIssue});

            line.Issues.Count.Should().Be(1);
            line.Issues.First().Should().Be(TestIssue);
        }
    }
}