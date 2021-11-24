using System;
using System.Collections.Generic;
using ChangeBlog.Application.ChangeLogLineParser;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.ServicesTests.ChangeLogLineParsing;

public class LineParsingRequestModelTests
{
    private IList<string> _testIssues;
    private IList<string> _testLabels;
    private string _testText;

    public LineParsingRequestModelTests()
    {
        _testText = "some bug fixes";
        _testLabels = new List<string>(0);
        _testIssues = new List<string>(0);
    }

    private LineParserRequestModel CreateRequestModel()
    {
        return new LineParserRequestModel(_testText, _testLabels, _testIssues);
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
    public void Create_NullText_ArgumentNullException()
    {
        _testText = null;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Create_NullIssues_ArgumentNullException()
    {
        _testIssues = null;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Create_NullLabels_ArgumentNullException()
    {
        _testLabels = null;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentNullException>();
    }
}