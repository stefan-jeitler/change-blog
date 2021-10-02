using System;
using System.Collections.Generic;
using System.Linq;
using ChangeBlog.Application.ChangeLogLineParser;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.ServicesTests.ChangeLogLineParsing
{
    public class LineParserTests
    {
        private readonly Mock<ILineParserOutput> _outputPortMock;

        public LineParserTests()
        {
            _outputPortMock = new Mock<ILineParserOutput>(MockBehavior.Strict);
        }

        [Fact]
        public void ParseChangeLogLine_ValidIssues_ReturnsIssuesAndNoOutput()
        {
            // arrange
            var issues = new List<string> {"#1234"};
            const string text = "some feature added";
            var lineParsingRequestModel = new LineParserRequestModel(text, Array.Empty<string>(), issues);

            // act
            var parseLine = LineParser.Parse(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            parseLine.HasValue.Should().BeTrue();
            parseLine.GetValueOrThrow().Issues.Count.Should().Be(1);
            parseLine.GetValueOrThrow().Issues.Should().Contain(Issue.Parse("#1234"));
        }

        [Fact]
        public void ParseChangeLogLine_InvalidIssue_InvalidIssuesOutput()
        {
            // arrange
            var issues = new List<string> {"#1234", "# 345"};
            const string text = "some feature added";
            var lineParsingRequestModel = new LineParserRequestModel(text, Array.Empty<string>(), issues);
            _outputPortMock.Setup(m => m.InvalidIssue(It.IsAny<string>(), It.IsAny<string>()));

            // act
            var parseLine = LineParser.Parse(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidIssue(It.Is<string>(x => x == text), It.Is<string>(x => x == "# 345")), Times.Once);
            parseLine.HasValue.Should().BeFalse();
        }

        [Fact]
        public void ParseChangeLogLine_TooManyIssues_TooManyIssuesOutput()
        {
            // arrange
            var issues = new List<string> {"#1", "#2", "#3", "#4", "#5", "#6", "#7", "#8", "#9", "#10", "#11"};
            const string text = "some feature added";
            var lineParsingRequestModel = new LineParserRequestModel(text, Array.Empty<string>(), issues);
            _outputPortMock.Setup(m => m.TooManyIssues(It.IsAny<string>(), It.IsAny<int>()));

            // act
            var parseLine = LineParser.Parse(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            _outputPortMock.Verify(m
                => m.TooManyIssues(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            parseLine.HasValue.Should().BeFalse();
        }

        [Fact]
        public void ParseChangeLogLine_ValidLabels_ReturnsExtractedLabelsAndNoOutput()
        {
            // arrange
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            const string text = "some feature added";
            var lineParsingRequestModel = new LineParserRequestModel(text, labels, Array.Empty<string>());

            // act
            var parseLine = LineParser.Parse(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            parseLine.HasValue.Should().BeTrue();
            parseLine.GetValueOrThrow().Labels.Count.Should().Be(2);
            parseLine.GetValueOrThrow().Labels.Should().Contain(Label.Parse("Bugfix"));
            parseLine.GetValueOrThrow().Labels.Should().Contain(Label.Parse("ProxyIssue"));
        }

        [Fact]
        public void ParseChangeLogLine_InvalidChangeLogLabel_InvalidLabelsOutput()
        {
            // arrange
            var labels = new List<string> {"Bugfix", "ProxyIssue", "invalid label"};
            const string text = "some feature added";
            var lineParsingRequestModel = new LineParserRequestModel(text, labels, Array.Empty<string>());
            _outputPortMock.Setup(m => m.InvalidLabel(It.IsAny<string>(), It.IsAny<string>()));

            // act
            var parseLine = LineParser.Parse(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidLabel(It.Is<string>(x => x == text),
                    It.Is<string>(x => x == labels.Last())), Times.Once);

            parseLine.HasValue.Should().BeFalse();
        }

        [Fact]
        public void ParseChangeLogLine_TooManyChangeLogLabels_TooManyLabelsOutput()
        {
            // arrange
            var labels = new List<string>
                {"Bugfix", "ProxyIssue", "Security", "ProxyStrikesBack", "Deprecated", "Feature"};
            const string text = "some feature added";
            var lineParsingRequestModel = new LineParserRequestModel(text, labels, Array.Empty<string>());
            _outputPortMock.Setup(m => m.TooManyLabels(It.IsAny<string>(), It.IsAny<int>()));

            // act
            var parseLine = LineParser.Parse(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            _outputPortMock.Verify(m
                    => m.TooManyLabels(It.Is<string>(x => x == text),
                        It.Is<int>(x => x == ChangeLogLine.MaxLabels)),
                Times.Once);

            parseLine.HasValue.Should().BeFalse();
        }


        [Fact]
        public void ParseChangeLogLine_InvalidLineText_InvalidChangeLogLineOutput()
        {
            // arrange
            const string text = "a";
            var lineParsingRequestModel =
                new LineParserRequestModel(text, Array.Empty<string>(), Array.Empty<string>());
            _outputPortMock.Setup(m => m.InvalidChangeLogLineText(It.IsAny<string>()));

            // act
            var parsedLine = LineParser.Parse(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidChangeLogLineText(It.Is<string>(x => x == text)), Times.Once);
            parsedLine.HasValue.Should().BeFalse();
        }
    }
}
