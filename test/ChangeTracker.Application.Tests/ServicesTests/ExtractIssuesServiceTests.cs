using System.Collections.Generic;
using System.Linq;
using ChangeTracker.Application.Services.Issues;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.ServicesTests
{
    public class ExtractIssuesServiceTests
    {
        private readonly Mock<IExtractIssuesOutputPort> _outputPortMock;

        public ExtractIssuesServiceTests()
        {
            _outputPortMock = new Mock<IExtractIssuesOutputPort>(MockBehavior.Strict);
        }

        [Fact]
        public void ExtractIssues_ValidIssues_ReturnsIssuesAndNoOutput()
        {
            // arrange
            var issues = new List<string> {"#1234"};
            var text = ChangeLogText.Parse("some feature added");

            // act
            var extractedIssues = ExtractIssuesService.Extract(_outputPortMock.Object, issues, text);

            // assert
            extractedIssues.HasValue.Should().BeTrue();
            extractedIssues.Value.Should().Contain(Issue.Parse("#1234"));
        }

        [Fact]
        public void ExtractIssues_InvalidIssue_InvalidIssuesOutput()
        {
            // arrange
            var issues = new List<string> {"#1234", "# 345"};
            var text = ChangeLogText.Parse("some feature added");
            _outputPortMock.Setup(m => m.InvalidIssue(It.IsAny<string>(), It.IsAny<string>()));

            // act
            var extractedIssues = ExtractIssuesService.Extract(_outputPortMock.Object, issues, text);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidIssue(It.Is<string>(x => x == text.Value), It.Is<string>(x => x == "# 345")), Times.Once);
            extractedIssues.HasValue.Should().BeFalse();
        }

        [Fact]
        public void ExtractIssues_TooManyIssues_TooManyIssuesOutput()
        {
            // arrange
            var issues = new List<string> {"#1", "#2", "#3", "#4", "#5", "#6", "#7", "#8", "#9", "#10", "#11"};
            var text = ChangeLogText.Parse("some feature added");
            _outputPortMock.Setup(m => m.TooManyIssues(It.IsAny<string>(), It.IsAny<int>()));

            // act
            var extractedIssues = ExtractIssuesService.Extract(_outputPortMock.Object, issues, text);

            // assert
            _outputPortMock.Verify(m
                => m.TooManyIssues(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            extractedIssues.HasValue.Should().BeFalse();
        }
    }
}