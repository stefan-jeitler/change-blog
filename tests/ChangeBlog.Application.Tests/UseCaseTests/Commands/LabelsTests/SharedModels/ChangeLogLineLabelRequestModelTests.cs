using System;
using ChangeBlog.Application.UseCases.Commands.Labels.SharedModels;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.LabelsTests.SharedModels;

public class ChangeLogLineLabelRequestModelTests
{
    [Fact]
    public void Create_WithEmptyLineId_ArgumentException()
    {
        var lineId = Guid.Empty;

        var act = () => new ChangeLogLineLabelRequestModel(lineId, "someLabel");

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullLabel_ArgumentNullException()
    {
        var act = () => new ChangeLogLineLabelRequestModel(Guid.NewGuid(), null);

        act.Should().ThrowExactly<ArgumentNullException>();
    }
}