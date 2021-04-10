using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.Services.Issues;
using ChangeTracker.Application.Services.Labels;
using ChangeTracker.Application.Services.NotReleasedVersion;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AddChangeLogLine;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
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
            var issues = new List<string> { "#1234" };
            var extractIssuesService = new ExtractIssuesService(_outputPortMock.Object);

            // act
            var extractedIssues = extractIssuesService.Extract(issues);

            // assert
            extractedIssues.HasValue.Should().BeTrue();
            extractedIssues.Value.Should().Contain(Issue.Parse("#1234"));
        }

        [Fact]
        public void ExtractIssues_InvalidIssue_InvalidIssuesOutput()
        {
            // arrange
            var issues = new List<string> { "#1234", "# 345" };
            _outputPortMock.Setup(m => m.InvalidIssues(It.IsAny<List<string>>()));
            var extractIssuesService = new ExtractIssuesService(_outputPortMock.Object);

            // act
            var extractedIssues = extractIssuesService.Extract(issues);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidIssues(It.Is<List<string>>(x => x.Count == 1 &&
                                                            x.First() == "# 345")), Times.Once);
            extractedIssues.HasValue.Should().BeFalse();
        }

        [Fact]
        public void ExtractIssues_TooManyIssues_TooManyIssuesOutput()
        {
            // arrange
            var issues = new List<string> { "#1", "#2", "#3", "#4", "#5", "#6", "#7", "#8", "#9", "#10", "#11" };
            _outputPortMock.Setup(m => m.TooManyIssues(It.IsAny<int>()));
            var extractIssuesService = new ExtractIssuesService(_outputPortMock.Object);

            // act
            var extractedIssues = extractIssuesService.Extract(issues);

            // assert
            _outputPortMock.Verify(m
                => m.TooManyIssues(It.Is<int>(x => x == ChangeLogLine.MaxIssues)), Times.Once);
            extractedIssues.HasValue.Should().BeFalse();
        }
    }
}
