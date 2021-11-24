using System;
using System.Collections.Generic;
using ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.AddVersion;

public class ChangeLogLineRequestModelTests
{
    private List<string> _testIssues;
    private List<string> _testLabels;
    private string _testText;

    public ChangeLogLineRequestModelTests()
    {
        _testText = "some bug fixes";
        _testLabels = new List<string>(0);
        _testIssues = new List<string>(0);
    }

    private ChangeLogLineRequestModel CreateRequestModel()
    {
        return new ChangeLogLineRequestModel(_testText, _testLabels, _testIssues);
    }

    [Fact]
    public void Create_HappyPath_Successful()
    {
        var requestModel = CreateRequestModel();

        requestModel.Text.Should().Be(_testText);
        requestModel.Labels.Should().BeEmpty();
        requestModel.Issues.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithNullText_ArgumentNullException()
    {
        _testText = null;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithNullLabels_ArgumentNullException()
    {
        _testLabels = null;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithNullIssues_ArgumentNullException()
    {
        _testIssues = null;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentNullException>();
    }
}