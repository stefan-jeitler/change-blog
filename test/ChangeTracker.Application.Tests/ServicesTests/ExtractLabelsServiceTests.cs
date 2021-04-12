using System.Collections.Generic;
using System.Linq;
using ChangeTracker.Application.Services.Labels;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.ServicesTests
{
    public class ExtractLabelsServiceTests
    {
        private readonly Mock<IExtractLabelsOutputPort> _outputPortMock;

        public ExtractLabelsServiceTests()
        {
            _outputPortMock = new Mock<IExtractLabelsOutputPort>(MockBehavior.Strict);
        }

        [Fact]
        public void ExtractLabels_ValidLabels_ReturnsExtractedLabelsAndNoOutput()
        {
            // arrange
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var text = ChangeLogText.Parse("some feature added");

            // act
            var extractedLabels = ExtractLabelsService.Extract(_outputPortMock.Object, labels, text);

            // assert
            extractedLabels.HasValue.Should().BeTrue();
            extractedLabels.Value.Count.Should().Be(2);
            extractedLabels.Value.Should().Contain(Label.Parse("Bugfix"));
            extractedLabels.Value.Should().Contain(Label.Parse("ProxyIssue"));
        }

        [Fact]
        public void ExtractLabels_InvalidChangeLogLabel_InvalidLabelsOutput()
        {
            // arrange
            var labels = new List<string> {"Bugfix", "ProxyIssue", "invalid label"};
            var text = ChangeLogText.Parse("some feature added");

            _outputPortMock.Setup(m => m.InvalidLabel(It.IsAny<string>(), It.IsAny<string>()));

            // act
            var extractedLabels = ExtractLabelsService.Extract(_outputPortMock.Object, labels, text);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidLabel(It.Is<string>(x => x == text.Value),
                    It.Is<string>(x => x == labels.Last())), Times.Once);

            extractedLabels.HasValue.Should().BeFalse();
        }

        [Fact]
        public void AddChangeLogLine_TooManyChangeLogLabels_TooManyLabelsOutput()
        {
            // arrange
            var labels = new List<string>
                {"Bugfix", "ProxyIssue", "Security", "ProxyStrikesBack", "Deprecated", "Feature"};

            var text = ChangeLogText.Parse("some feature added");
            _outputPortMock.Setup(m => m.TooManyLabels(It.IsAny<string>(), It.IsAny<int>()));

            // act
            var extracted = ExtractLabelsService.Extract(_outputPortMock.Object, labels, text);

            // assert
            _outputPortMock.Verify(m
                => m.TooManyLabels(It.Is<string>(x => x == text.Value), It.Is<int>(x => x == ChangeLogLine.MaxLabels)), Times.Once);

            extracted.HasValue.Should().BeFalse();
        }
    }
}