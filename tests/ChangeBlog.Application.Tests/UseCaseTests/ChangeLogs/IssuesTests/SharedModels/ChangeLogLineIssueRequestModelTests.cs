using System;
using ChangeBlog.Application.UseCases.ChangeLogs.Issues.SharedModels;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.ChangeLogs.IssuesTests.SharedModels;

public class ChangeLogLineIssueRequestModelTests
{
    [Fact]
    public void Create_WithEmptyLineId_ArgumentException()
    {
        var lineId = Guid.Empty;

        var act = () => new ChangeLogLineIssueRequestModel(lineId, "#1234");

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullIssue_ArgumentNullException()
    {
        var act = () => new ChangeLogLineIssueRequestModel(Guid.NewGuid(), null);

        act.Should().ThrowExactly<ArgumentNullException>();
    }
}