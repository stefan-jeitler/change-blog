using System;
using System.Collections.Generic;
using System.Linq;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

// ReSharper disable InconsistentNaming

namespace ChangeTracker.Domain.Tests.ChangeLogTests
{
    public class ChangeLogLineIssuesTests
    {
        private const int TestPosition = 5;
        private static readonly Issue TestIssue = Issue.Parse("#1234");
        private static readonly Guid TestId = Guid.Parse("51d89265-52c2-4a38-a0fe-b99bdc5523d0");
        private static readonly Guid TestVersionId = Guid.Parse("66845d0a-45bc-4834-96d0-b48c2c403628");
        private static readonly Guid TestProductId = Guid.Parse("ef5656e5-15f0-418d-b3a4-b69f1c3abac5");
        private static readonly ChangeLogText TestText = ChangeLogText.Parse("New feature added");
        private static readonly Guid TestUserId = Guid.Parse("294c4f04-85d4-4d5b-ae25-e6b618f1676f");
        private static readonly DateTime TestCreationDate = DateTime.Parse("2021-04.02T18:28");

        [Fact]
        public void Create_WithIssues_IssuesExists()
        {
            var line = new ChangeLogLine(TestId,
                TestVersionId,
                TestProductId,
                TestText,
                TestPosition,
                TestCreationDate,
                Array.Empty<Label>(),
                new List<Issue> {TestIssue},
                TestUserId);

            line.Issues.Count.Should().Be(1);
            line.Issues.First().Should().Be(TestIssue);
        }

        [Fact]
        public void Create_WithoutIssues_Empty()
        {
            var line = new ChangeLogLine(TestId,
                TestVersionId,
                TestProductId,
                TestText,
                TestPosition,
                TestCreationDate,
                Enumerable.Empty<Label>(),
                Enumerable.Empty<Issue>(),
                TestUserId);

            line.Issues.Should().BeEmpty();
        }


        [Fact]
        public void Create_WithNullIssues_ArgumentNullException()
        {
            Func<ChangeLogLine> act = () => new ChangeLogLine(TestId,
                TestVersionId,
                TestProductId,
                TestText,
                TestPosition,
                TestCreationDate,
                Enumerable.Empty<Label>(),
                null,
                TestUserId);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithTooManyIssues_ArgumentException()
        {
            var issues = new[]
            {
                Issue.Parse("#1231"), Issue.Parse("#1232"), Issue.Parse("#1233"),
                Issue.Parse("#1234"), Issue.Parse("#1235"), Issue.Parse("#1236"),
                Issue.Parse("#1237"), Issue.Parse("#1238"), Issue.Parse("#1239"),
                Issue.Parse("#12310"), Issue.Parse("#12311")
            };

            Func<ChangeLogLine> act = () => new ChangeLogLine(TestId,
                TestVersionId,
                TestProductId,
                TestText,
                TestPosition,
                TestCreationDate,
                Enumerable.Empty<Label>(),
                issues,
                TestUserId);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithMaxIssues_SuccessfulCreated()
        {
            var issues = new[]
            {
                Issue.Parse("#1231"), Issue.Parse("#1232"), Issue.Parse("#1233"),
                Issue.Parse("#1234"), Issue.Parse("#1235"), Issue.Parse("#1236"),
                Issue.Parse("#1237"), Issue.Parse("#1238"), Issue.Parse("#1239"),
                Issue.Parse("#12310")
            };

            var line = new ChangeLogLine(TestId,
                TestVersionId,
                TestProductId,
                TestText,
                TestPosition,
                TestCreationDate,
                Enumerable.Empty<Label>(),
                issues,
                TestUserId);

            line.Issues.Count.Should().Be(10);
        }

        [Fact]
        public void AddIssue_ToLine_SuccessfullyAdded()
        {
            // arrange
            var issue = Issue.Parse("#1234");
            var line = new ChangeLogLine(TestId,
                null, TestProductId, TestText,
                TestPosition, TestCreationDate,
                Enumerable.Empty<Label>(),
                Enumerable.Empty<Issue>(),
                TestUserId);

            // act
            line.AddIssue(issue);

            // assert
            line.Issues.Should().Contain(issue);
        }

        [Fact]
        public void AddIssue_MaxIssuesReached_ArgumentException()
        {
            // arrange
            var issues = new[]
            {
                Issue.Parse("#12341"), Issue.Parse("#12342"), Issue.Parse("#12343"),
                Issue.Parse("#12344"), Issue.Parse("#12345"), Issue.Parse("#12346"),
                Issue.Parse("#12347"), Issue.Parse("#12348"), Issue.Parse("#12349"),
                Issue.Parse("#123410")
            };

            var line = new ChangeLogLine(TestId,
                null, TestProductId, TestText,
                TestPosition, TestCreationDate,
                Enumerable.Empty<Label>(), issues,
                TestUserId);

            // act
            Action act = () => line.AddIssue(Issue.Parse("#123411"));

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void RemoveIssue_NoIssuesExists_NothingChanged()
        {
            var line = new ChangeLogLine(TestId,
                null, TestProductId, TestText,
                TestPosition, TestCreationDate,
                Enumerable.Empty<Label>(),
                Enumerable.Empty<Issue>(),
                TestUserId);

            line.RemoveIssue(Issue.Parse("#123411"));

            line.Issues.Should().BeEmpty();
        }

        [Fact]
        public void RemoveIssue_ThatExists_IssueRemoved()
        {
            var issue = Issue.Parse("#123411");
            var line = new ChangeLogLine(TestId,
                null, TestProductId, TestText,
                TestPosition, TestCreationDate,
                Enumerable.Empty<Label>(),
                new List<Issue>(1) {issue},
                TestUserId);

            line.RemoveIssue(issue);

            line.Issues.Should().BeEmpty();
        }

        [Fact]
        public void RemoveIssue_TwoExists_OneGetsRemoved()
        {
            var issue1 = Issue.Parse("#123411");
            var issue2 = Issue.Parse("#123412");
            var line = new ChangeLogLine(TestId,
                null, TestProductId, TestText,
                TestPosition, TestCreationDate,
                Enumerable.Empty<Label>(),
                new List<Issue>(1) {issue1, issue2},
                TestUserId);

            line.RemoveIssue(issue2);

            line.Issues.Should().Contain(issue1);
            line.Issues.Count.Should().Be(1);
        }

        [Fact]
        public void AvailableIssuePlaces_NoIssueExists_ReturnsMaxIssuesPerLine()
        {
            var line = new ChangeLogLine(TestId,
                TestVersionId, TestProductId, TestText,
                TestPosition, TestCreationDate,
                Enumerable.Empty<Label>(), Enumerable.Empty<Issue>(),
                TestUserId);

            var remainingIssuePlaces = line.AvailableIssuePlaces;

            remainingIssuePlaces.Should().Be(ChangeLogLine.MaxIssues);
        }

        [Fact]
        public void AvailableIssuePlaces_TenIssueExists_ReturnsZero()
        {
            // arrange
            var issues = new[]
            {
                Issue.Parse("#12341"), Issue.Parse("#12342"), Issue.Parse("#12343"),
                Issue.Parse("#12344"), Issue.Parse("#12345"), Issue.Parse("#12346"),
                Issue.Parse("#12347"), Issue.Parse("#12348"), Issue.Parse("#12349"),
                Issue.Parse("#123410")
            };

            var line = new ChangeLogLine(TestId,
                null, TestProductId, TestText,
                TestPosition, TestCreationDate,
                Enumerable.Empty<Label>(), issues,
                TestUserId);

            // act
            var remainingIssuePlaces = line.AvailableIssuePlaces;

            // assert
            remainingIssuePlaces.Should().Be(0);
        }
    }
}