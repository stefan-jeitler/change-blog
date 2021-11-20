using System;
using System.Collections.Generic;
using ChangeBlog.Application.UseCases.Commands.DeleteChangeLogLine;
using ChangeBlog.Application.UseCases.Commands.UpdateChangeLogLine;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.UpdateChangeLogLine;

public class ChangeLogLineRequestModelTests
{
    private readonly ChangeLogLineType _changeLogLineType;
    private readonly IList<string> _testIssues;
    private readonly IList<string> _testLabels;
    private readonly string _testText;
    private Guid _testLineId;

    public ChangeLogLineRequestModelTests()
    {
        _testLineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        _testText = "some features added";
        _testLabels = Array.Empty<string>();
        _testIssues = Array.Empty<string>();
        _changeLogLineType = ChangeLogLineType.NotPending;
    }

    private UpdateChangeLogLineRequestModel CreateRequestModel()
    {
        return new(_testLineId, _changeLogLineType, _testText, _testLabels, _testIssues);
    }

    [Fact]
    public void Create_HappyPath_Successful()
    {
        var requestModel = CreateRequestModel();

        requestModel.ChangeLogLineId.Should().Be(_testLineId);
        requestModel.Text.Should().Be(_testText);
        requestModel.Labels.Should().BeEmpty();
        requestModel.Issues.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithEmptyLineId_ArgumentException()
    {
        _testLineId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
    }
}