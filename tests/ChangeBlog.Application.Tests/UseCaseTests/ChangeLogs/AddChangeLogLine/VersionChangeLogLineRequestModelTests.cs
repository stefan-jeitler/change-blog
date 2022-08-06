using System;
using System.Collections.Generic;
using ChangeBlog.Application.UseCases.ChangeLogs.AddChangeLogLine.Models;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.ChangeLogs.AddChangeLogLine;

public class VersionChangeLogLineRequestModelTests
{
    private static Guid _testUserId;
    private List<string> _testIssues;
    private List<string> _testLabels;
    private Guid _testProductId;
    private string _testText;
    private string _testVersion;

    public VersionChangeLogLineRequestModelTests()
    {
        _testProductId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
        _testUserId = Guid.Parse("294c4f04-85d4-4d5b-ae25-e6b618f1676f");
        _testVersion = "1.2.3";
        _testText = "some bug fixes";
        _testLabels = new List<string>(0);
        _testIssues = new List<string>(0);
    }

    private VersionChangeLogLineRequestModel CreateRequestModel()
    {
        return new VersionChangeLogLineRequestModel(_testUserId, _testProductId,
            _testVersion, _testText, _testLabels, _testIssues);
    }

    [Fact]
    public void Create_HappyPath_Successful()
    {
        var requestModel = CreateRequestModel();

        requestModel.ProductId.Should().Be(_testProductId);
        requestModel.Version.Should().Be(_testVersion);
        requestModel.Text.Should().Be(_testText);
        requestModel.Labels.Should().BeEmpty();
        requestModel.Issues.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithEmptyProductId_ArgumentException()
    {
        _testProductId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullVersion_ArgumentNullException()
    {
        _testVersion = null;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentNullException>();
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