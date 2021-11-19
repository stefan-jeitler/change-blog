using System;
using System.Collections.Generic;
using ChangeBlog.Application.UseCases.Commands.AddChangeLogLine.Models;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.AddChangeLogLine;

public class VersionIdChangeLogLineRequestModelTests
{
    private static Guid _testUserId;
    private List<string> _testIssues;
    private List<string> _testLabels;
    private string _testText;
    private Guid _testVersionId;

    public VersionIdChangeLogLineRequestModelTests()
    {
        _testVersionId = Guid.Parse("079bac32-3092-4620-90d6-6a4b3888664e");
        _testUserId = Guid.Parse("294c4f04-85d4-4d5b-ae25-e6b618f1676f");
        _testText = "some bug fixes";
        _testLabels = new List<string>(0);
        _testIssues = new List<string>(0);
    }

    private VersionIdChangeLogLineRequestModel CreateRequestModel() =>
        new(_testUserId, _testVersionId, _testText, _testLabels, _testIssues);

    [Fact]
    public void Create_HappyPath_Successful()
    {
        var requestModel = CreateRequestModel();

        requestModel.VersionId.Should().Be(_testVersionId);
        requestModel.Text.Should().Be(_testText);
        requestModel.Labels.Should().BeEmpty();
        requestModel.Issues.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithEmptyVersionId_ArgumentNullException()
    {
        _testVersionId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
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