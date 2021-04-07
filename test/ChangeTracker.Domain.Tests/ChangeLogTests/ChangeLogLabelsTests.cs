using System;
using System.Collections.Generic;
using System.Linq;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ChangeLogTests
{
    public class ChangeLogLabelsTests
    {
        private const uint TestPosition = 5;
        private static readonly Guid TestId = Guid.Parse("51d89265-52c2-4a38-a0fe-b99bdc5523d0");
        private static readonly Guid TestVersionId = Guid.Parse("66845d0a-45bc-4834-96d0-b48c2c403628");
        private static readonly Guid TestProjectId = Guid.Parse("ef5656e5-15f0-418d-b3a4-b69f1c3abac5");
        private static readonly ChangeLogText TestText = ChangeLogText.Parse("New feature added");
        private static readonly DateTime TestCreationDate = DateTime.Parse("2021-04.02T18:28");

        [Fact]
        public void Create_WithLabels_LabelsExists()
        {
            // arrange
            var featureLabel = Label.Parse("Feature");
            var bugLabel = Label.Parse("Bug");
            var labels = new List<Label>
            {
                featureLabel,
                bugLabel
            };

            // act
            var line = new ChangeLogLine(TestId, TestVersionId,
                TestProjectId, TestText, TestPosition,
                TestCreationDate, labels);

            // assert
            line.Labels.Count.Should().Be(2);
            line.Labels.Should().Contain(featureLabel);
            line.Labels.Should().Contain(bugLabel);
        }

        [Fact]
        public void Create_WithNullArgument_EmptyList()
        {
            var line = new ChangeLogLine(TestId, TestVersionId,
                TestProjectId, TestText, TestPosition,
                TestCreationDate);

            // assert
            line.Labels.Should().BeEmpty();
        }

        [Fact]
        public void Create_With6Labels_OnlyFirst5Taken()
        {
            var labels = new List<Label>
            {
                Label.Parse("Added"), Label.Parse("Changed"),
                Label.Parse("Deprecated"), Label.Parse("Removed"),
                Label.Parse("Fixed"), Label.Parse("Security")
            };

            var line = new ChangeLogLine(TestId, TestVersionId,
                TestProjectId, TestText, TestPosition,
                TestCreationDate, labels);

            // assert
            line.Labels.Count.Should().Be(5);
            line.Labels.Should().NotContain(Label.Parse("Security"));
        }

        [Fact]
        public void Create_WithoutLabels_EmptyListExists()
        {
            var line = new ChangeLogLine(TestId, TestVersionId,
                TestProjectId, TestText, TestPosition,
                TestCreationDate);

            line.Labels.Should().BeEmpty();
        }

        [Fact]
        public void TryAdd_LabelToPendingNote_LabelAdded()
        {
            var featureLabel = Label.Parse("Feature");
            var line = new ChangeLogLine(TestId, null,
                TestProjectId, TestText, TestPosition,
                TestCreationDate);

            var isAdded = line.TryAddLabel(featureLabel);

            line.Labels.Count.Should().Be(1);
            isAdded.Should().Be(true);
        }

        [Fact]
        public void TryAdd_LabelToNotPendingNote_NotAdded()
        {
            var featureLabel = Label.Parse("Feature");
            var line = new ChangeLogLine(TestId, TestVersionId,
                TestProjectId, TestText, TestPosition,
                TestCreationDate);

            var isAdded = line.TryAddLabel(featureLabel);

            line.Labels.Should().BeEmpty();
            isAdded.Should().BeFalse();
        }

        [Fact]
        public void TryAdd_LabelWhenMaxCountIsReached_NotAdded()
        {
            // arrange
            var labels = new List<Label>
            {
                Label.Parse("Added"), Label.Parse("Changed"),
                Label.Parse("Deprecated"), Label.Parse("Removed"),
                Label.Parse("Fixed")
            };

            var line = new ChangeLogLine(TestId, null,
                TestProjectId, TestText, TestPosition,
                TestCreationDate, labels);

            var newLabel = Label.Parse("Security");

            // act
            var isAdded = line.TryAddLabel(newLabel);

            // assert
            isAdded.Should().BeFalse();
            line.Labels.Count.Should().Be(5);
            line.Labels.Should().NotContain(newLabel);
        }

        [Fact]
        public void TryAdd_ToExistingLabels_NewLabelAdded()
        {
            // arrange
            var featureLabel = Label.Parse("Feature");
            var bugLabel = Label.Parse("Bug");
            var deprecatedLabel = Label.Parse("Deprecated");
            var existingLabels = new List<Label> {featureLabel, bugLabel};

            var line = new ChangeLogLine(TestId, null,
                TestProjectId, TestText, TestPosition,
                TestCreationDate, existingLabels);

            // act
            var isAdded = line.TryAddLabel(deprecatedLabel);

            // assert
            isAdded.Should().Be(true);
            line.Labels.Count.Should().Be(3);
            line.Labels.Should().Contain(featureLabel);
            line.Labels.Should().Contain(bugLabel);
            line.Labels.Should().Contain(deprecatedLabel);
        }

        [Fact]
        public void Create_WithSameLabels_DuplicatesRemoved()
        {
            // arrange
            var featureLabel = Label.Parse("Feature");
            var existingLabels = new List<Label> {featureLabel, featureLabel};

            // act
            var line = new ChangeLogLine(TestId, TestVersionId,
                TestProjectId, TestText, TestPosition,
                TestCreationDate, existingLabels);

            // assert
            line.Labels.Count.Should().Be(1);
            line.Labels.First().Should().Be(featureLabel);
        }

        [Fact]
        public void TryAdd_SameLabel_NoDuplicates()
        {
            // arrange
            var featureLabel = Label.Parse("Feature");
            var existingLabels = new List<Label> {featureLabel};

            var line = new ChangeLogLine(TestId, TestVersionId,
                TestProjectId, TestText, TestPosition,
                TestCreationDate, existingLabels);

            // act
            var isAdded = line.TryAddLabel(featureLabel);

            // assert
            isAdded.Should().BeFalse();
            line.Labels.Count.Should().Be(1);
            line.Labels.First().Should().Be(featureLabel);
        }

        [Fact]
        public void TryRemove_ExistingLabel_LabelRemoved()
        {
            // arrange
            var featureLabel = Label.Parse("Feature");
            var existingLabels = new List<Label> {featureLabel};

            var line = new ChangeLogLine(TestId, null,
                TestProjectId, TestText, TestPosition,
                TestCreationDate, existingLabels);

            // act
            var isRemoved = line.TryRemoveLabel(featureLabel);

            // assert
            isRemoved.Should().BeTrue();
            line.Labels.Should().BeEmpty();
        }

        [Fact]
        public void TryRemove_NoLabelExists_ReturnsFalse()
        {
            // arrange
            var line = new ChangeLogLine(TestId, TestVersionId,
                TestProjectId, TestText, TestPosition,
                TestCreationDate);

            // act
            var isRemoved = line.TryRemoveLabel(Label.Parse("Feature"));

            // assert
            isRemoved.Should().BeFalse();
            line.Labels.Should().BeEmpty();
        }


        [Fact]
        public void TryRemove_NotPendingChangeLogLine_LabelNotRemoved()
        {
            // arrange
            var featureLabel = Label.Parse("Feature");
            var existingLabels = new List<Label> {featureLabel};

            var line = new ChangeLogLine(TestId, TestVersionId,
                TestProjectId, TestText, TestPosition,
                TestCreationDate, existingLabels);

            // act
            var isRemoved = line.TryRemoveLabel(featureLabel);

            // assert
            isRemoved.Should().BeFalse();
            line.Labels.Should().Contain(featureLabel);
        }
    }
}