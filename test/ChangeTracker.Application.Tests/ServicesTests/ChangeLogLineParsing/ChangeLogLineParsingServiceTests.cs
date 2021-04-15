using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.ServicesTests.ChangeLogLineParsing
{
    public class ChangeLogLineParsingServiceTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IChangeLogLineParsingOutput> _outputPortMock;

        public ChangeLogLineParsingServiceTests()
        {
            _changeLogDaoStub = new ChangeLogDaoStub();
            _outputPortMock = new Mock<IChangeLogLineParsingOutput>(MockBehavior.Strict);
        }

        [Fact]
        public async Task ParseChangeLogLine_ValidIssues_ReturnsIssuesAndNoOutput()
        {
            // arrange
            var issues = new List<string> {"#1234"};
            const string text = "some feature added";
            var service = new ChangeLogLineParsingService(_changeLogDaoStub);
            var lineParsingRequestModel = new LineParsingRequestModel(TestAccount.Project.Id,
                null, text, Array.Empty<string>(), issues, 0);

            // act
            var parseLine = await service.ParseAsync(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            parseLine.HasValue.Should().BeTrue();
            parseLine.Value.Issues.Count.Should().Be(1);
            parseLine.Value.Issues.Should().Contain(Issue.Parse("#1234"));
        }

        [Fact]
        public async Task ParseChangeLogLine_InvalidIssue_InvalidIssuesOutput()
        {
            // arrange
            var issues = new List<string> {"#1234", "# 345"};
            const string text = "some feature added";
            var service = new ChangeLogLineParsingService(_changeLogDaoStub);
            var lineParsingRequestModel = new LineParsingRequestModel(TestAccount.Project.Id,
                null, text, Array.Empty<string>(), issues, 0);
            _outputPortMock.Setup(m => m.InvalidIssue(It.IsAny<string>(), It.IsAny<string>()));

            // act
            var parseLine = await service.ParseAsync(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidIssue(It.Is<string>(x => x == text), It.Is<string>(x => x == "# 345")), Times.Once);
            parseLine.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task ParseChangeLogLine_TooManyIssues_TooManyIssuesOutput()
        {
            // arrange
            var issues = new List<string> {"#1", "#2", "#3", "#4", "#5", "#6", "#7", "#8", "#9", "#10", "#11"};
            const string text = "some feature added";
            var service = new ChangeLogLineParsingService(_changeLogDaoStub);
            var lineParsingRequestModel = new LineParsingRequestModel(TestAccount.Project.Id,
                null, text, Array.Empty<string>(), issues, 0);
            _outputPortMock.Setup(m => m.TooManyIssues(It.IsAny<string>(), It.IsAny<int>()));

            // act
            var parseLine = await service.ParseAsync(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            _outputPortMock.Verify(m
                => m.TooManyIssues(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            parseLine.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task ParseChangeLogLine_ValidLabels_ReturnsExtractedLabelsAndNoOutput()
        {
            // arrange
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            const string text = "some feature added";
            var service = new ChangeLogLineParsingService(_changeLogDaoStub);
            var lineParsingRequestModel = new LineParsingRequestModel(TestAccount.Project.Id,
                null, text, labels, Array.Empty<string>(), 0);

            // act
            var parseLine = await service.ParseAsync(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            parseLine.HasValue.Should().BeTrue();
            parseLine.Value.Labels.Count.Should().Be(2);
            parseLine.Value.Labels.Should().Contain(Label.Parse("Bugfix"));
            parseLine.Value.Labels.Should().Contain(Label.Parse("ProxyIssue"));
        }

        [Fact]
        public async Task ParseChangeLogLine_InvalidChangeLogLabel_InvalidLabelsOutput()
        {
            // arrange
            var labels = new List<string> {"Bugfix", "ProxyIssue", "invalid label"};
            const string text = "some feature added";
            var service = new ChangeLogLineParsingService(_changeLogDaoStub);
            var lineParsingRequestModel = new LineParsingRequestModel(TestAccount.Project.Id,
                null, text, labels, Array.Empty<string>(), 0);
            _outputPortMock.Setup(m => m.InvalidLabel(It.IsAny<string>(), It.IsAny<string>()));

            // act
            var parseLine = await service.ParseAsync(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidLabel(It.Is<string>(x => x == text),
                    It.Is<string>(x => x == labels.Last())), Times.Once);

            parseLine.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task ParseChangeLogLine_TooManyChangeLogLabels_TooManyLabelsOutput()
        {
            // arrange
            var labels = new List<string>
                {"Bugfix", "ProxyIssue", "Security", "ProxyStrikesBack", "Deprecated", "Feature"};
            const string text = "some feature added";
            var service = new ChangeLogLineParsingService(_changeLogDaoStub);
            var lineParsingRequestModel = new LineParsingRequestModel(TestAccount.Project.Id,
                null, text, labels, Array.Empty<string>(), 0);
            _outputPortMock.Setup(m => m.TooManyLabels(It.IsAny<string>(), It.IsAny<int>()));

            // act
            var parseLine = await service.ParseAsync(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            _outputPortMock.Verify(m
                    => m.TooManyLabels(It.Is<string>(x => x == text),
                        It.Is<int>(x => x == ChangeLogLine.MaxLabels)),
                Times.Once);

            parseLine.HasValue.Should().BeFalse();
        }


        [Fact]
        public async Task ParseChangeLogLine_InvalidLineText_InvalidChangeLogLineOutput()
        {
            // arrange
            const string text = "a";
            var service = new ChangeLogLineParsingService(_changeLogDaoStub);
            var lineParsingRequestModel = new LineParsingRequestModel(TestAccount.Project.Id,
                null, text, Array.Empty<string>(), Array.Empty<string>(), 0);
            _outputPortMock.Setup(m => m.InvalidChangeLogLineText(It.IsAny<string>()));

            // act
            var parsedLine = await service.ParseAsync(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidChangeLogLineText(It.Is<string>(x => x == text)), Times.Once);
            parsedLine.HasValue.Should().BeFalse();
        }


        [Fact]
        public async Task ParseChangeLogLine_NoMoreLinesAvailable_TooManyChangeLogLinesOutput()
        {
            // arrange
            const string text = "Some Bug fixed";
            var service = new ChangeLogLineParsingService(_changeLogDaoStub);
            var lineParsingRequestModel = new LineParsingRequestModel(TestAccount.Project.Id,
                null, text, Array.Empty<string>(), Array.Empty<string>());

            _changeLogDaoStub.ChangeLogs.AddRange(Enumerable.Range(0, 100)
                .Select(x => new ChangeLogLine(Guid.NewGuid(),
                    null,
                    TestAccount.Project.Id,
                    ChangeLogText.Parse($"{x:D5}"),
                    (uint) x,
                    DateTime.Parse("2021-04-09"))));

            _outputPortMock.Setup(m => m.TooManyLines(It.IsAny<int>()));

            // act
            var parseLine = await service.ParseAsync(_outputPortMock.Object, lineParsingRequestModel);

            // assert
            _outputPortMock.Verify(m => m.TooManyLines(
                It.Is<int>(x => x == ChangeLogsMetadata.MaxChangeLogLines)), Times.Once);
            parseLine.HasValue.Should().BeFalse();
        }
    }
}