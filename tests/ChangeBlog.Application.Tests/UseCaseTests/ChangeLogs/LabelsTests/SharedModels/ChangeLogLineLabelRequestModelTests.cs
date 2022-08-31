using System;
using ChangeBlog.Application.UseCases.ChangeLogs.Labels.SharedModels;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.ChangeLogs.LabelsTests.SharedModels;

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
        var lineId = Guid.Parse("d8de7d91-ad4b-4f37-a55c-5bc744c42f9b");
        
        var act = () => new ChangeLogLineLabelRequestModel(lineId, null);

        act.Should().ThrowExactly<ArgumentNullException>();
    }
}