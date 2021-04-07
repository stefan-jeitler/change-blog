﻿using System;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ChangeLogTests
{
    public class IssueTests
    {
        [Fact]
        public void Parse_WithValidIssueNumber_Successful()
        {
            var issue = Issue.Parse("#1234");

            issue.Value.Should().Be("#1234");
        }

        [Fact]
        public void Parse_WithNull_ArgumentNullException()
        {
            Func<Issue> act = () => Issue.Parse(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Parse_WithEmptyString_ArgumentException()
        {
            Func<Issue> act = () => Issue.Parse(string.Empty);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_WithWhitespace_ArgumentException()
        {
            Func<Issue> act = () => Issue.Parse(" ");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData(" #1234")]
        [InlineData(" #1234 ")]
        [InlineData("#1234 ")]
        public void Parse_WithLeadingAndTrailingWhitespaces_WhitespacesRemoved(string issueWithWhitespace)
        {
            var issue = Issue.Parse(issueWithWhitespace);

            issue.Value.Should().Be(issueWithWhitespace.Trim());
        }

        [Fact]
        public void Parse_WithTooLongIssueNumber_Max50CharactersAllowed()
        {
            var tooLongIssueNumber = new string('a', 51);

            Func<Issue> act = () => Issue.Parse(tooLongIssueNumber);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void TryParse_ValidIssueNumber_Successful()
        {
            var isSuccess = Issue.TryParse("#1234", out var issue);

            isSuccess.Should().BeTrue();
            issue.Value.Should().Be("#1234");
        }

        [Fact]
        public void TryParse_InvalidIssueNumber_ReturnsFalse()
        {
            var isSuccess = Issue.TryParse("", out var issue);

            isSuccess.Should().BeFalse();
            issue.Should().BeNull();
        }
    }
}