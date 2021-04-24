using System;
using ChangeTracker.Application.UseCases.Command.Issues.SharedModels;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.IssuesTests.SharedModels
{
    public class ChangeLogLineIssueRequestModelTests
    {
        [Fact]
        public void Create_WithEmptyLineId_ArgumentException()
        {
            var lineId = Guid.Empty;

            Func<ChangeLogLineIssueRequestModel> act = () => new ChangeLogLineIssueRequestModel(lineId, "#1234");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullIssue_ArgumentNullException()
        {
            Func<ChangeLogLineIssueRequestModel> act = () => new ChangeLogLineIssueRequestModel(Guid.NewGuid(), null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}