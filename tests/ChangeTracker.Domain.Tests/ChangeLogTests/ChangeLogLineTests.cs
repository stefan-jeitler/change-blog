using System;
using System.Collections.Generic;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

// ReSharper disable InconsistentNaming

namespace ChangeTracker.Domain.Tests.ChangeLogTests
{
    public class ChangeLogLineTests
    {
        private readonly List<Issue> _testIssues;
        private readonly List<Label> _testLabels;
        private readonly uint _testPosition;
        private DateTime _testCreationDate;
        private DateTime? _testDeletionDate;
        private Guid _testId;
        private Guid _testProductId;
        private ChangeLogText _testText;
        private Guid? _testVersionId;

        public ChangeLogLineTests()
        {
            _testPosition = 5;
            _testIssues = new List<Issue>(1) {Issue.Parse("#1234")};
            _testLabels = new List<Label>(1) {Label.Parse("Feature")};
            _testId = Guid.Parse("51d89265-52c2-4a38-a0fe-b99bdc5523d0");
            _testVersionId = Guid.Parse("66845d0a-45bc-4834-96d0-b48c2c403628");
            _testProductId = Guid.Parse("ef5656e5-15f0-418d-b3a4-b69f1c3abac5");
            _testText = ChangeLogText.Parse("New feature added");
            _testCreationDate = DateTime.Parse("2021-04-02T18:28");
            _testDeletionDate = null;
        }

        private ChangeLogLine CreateChangeLogLine()
        {
            return new(_testId,
                _testVersionId,
                _testProductId,
                _testText,
                _testPosition,
                _testCreationDate,
                _testLabels,
                _testIssues,
                _testDeletionDate);
        }

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var line = CreateChangeLogLine();

            line.Id.Should().Be(_testId);
            line.VersionId.Should().Be(_testVersionId);
            line.ProductId.Should().Be(_testProductId);
            line.Text.Should().Be(_testText);
            line.Position.Should().Be(_testPosition);
            line.CreatedAt.Should().Be(_testCreationDate);
            line.Labels.Count.Should().Be(1);
            line.Issues.Count.Should().Be(1);
            line.DeletedAt.Should().BeNull();
        }

        [Fact]
        public void Create_WithOverloadedConstructor_IdIsGenerated()
        {
            var line = new ChangeLogLine(_testVersionId, _testProductId, _testText, _testPosition);

            line.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void Create_WithDeletionDate_DateIsProperlySet()
        {
            _testDeletionDate = DateTime.Parse("2021-04-16");

            var line = CreateChangeLogLine();

            line.DeletedAt.Should().HaveValue();
            line.DeletedAt!.Value.Should().Be(_testDeletionDate!.Value);
        }

        [Fact]
        public void Create_WithOverloadedConstructor_IdAndCreatedAtDatetimeGenerated()
        {
            var line = new ChangeLogLine(_testVersionId, _testProductId, _testText, _testPosition);

            line.Id.Should().NotBe(Guid.Empty);
            line.CreatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
            line.CreatedAt.Should().BeBefore(DateTime.UtcNow);
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            _testId = Guid.Empty;

            Func<ChangeLogLine> act = CreateChangeLogLine;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyVersionId_Successful()
        {
            _testVersionId = Guid.Empty;

            Func<ChangeLogLine> act = CreateChangeLogLine;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersionId_IsPendingChangeLogLine()
        {
            _testVersionId = null;

            var line = CreateChangeLogLine();

            line.IsPending.Should().BeTrue();
        }

        [Fact]
        public void Create_WithEmptyProductId_ArgumentException()
        {
            _testProductId = Guid.Empty;

            Func<ChangeLogLine> act = CreateChangeLogLine;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullText_ArgumentNullException()
        {
            _testText = null;

            Func<ChangeLogLine> act = CreateChangeLogLine;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidCreatedAtDate_ArgumentException(string invalidDate)
        {
            _testCreationDate = DateTime.Parse(invalidDate);

            Func<ChangeLogLine> act = CreateChangeLogLine;

            act.Should().ThrowExactly<ArgumentException>();
        }


        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidDeletedAtDate_ArgumentException(string invalidDate)
        {
            _testDeletionDate = DateTime.Parse(invalidDate);

            Func<ChangeLogLine> act = CreateChangeLogLine;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void AssignToVersion_CurrentVersionIsNull_SuccessfullyAssigned()
        {
            // arrange
            var versionId = Guid.Parse("9102c1a0-09cf-4cb8-a6f6-fc0660be6109");
            _testVersionId = null;
            var line = CreateChangeLogLine();

            // act
            var assignedLine = line.AssignToVersion(versionId, 0);

            // assert
            assignedLine.VersionId.Should().Be(versionId);
        }

        [Fact]
        public void AssignToVersion_PositionProperlySet()
        {
            // arrange
            _testVersionId = null;
            var line = CreateChangeLogLine();

            // act
            var assignedLine = line.AssignToVersion(Guid.Parse("9102c1a0-09cf-4cb8-a6f6-fc0660be6109"), 2);

            // assert
            assignedLine.Position.Should().Be(2);
        }

        [Fact]
        public void AssignToVersion_NewVersionIsEmpty_ArgumentException()
        {
            // arrange
            _testVersionId = null;
            var line = CreateChangeLogLine();

            // act
            Func<ChangeLogLine> act = () => line.AssignToVersion(Guid.Empty, 0);

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void AssignToVersion_LineIsNotPending_ArgumentException()
        {
            // arrange
            var versionId = Guid.Parse("9102c1a0-09cf-4cb8-a6f6-fc0660be6109");
            _testVersionId = versionId;
            var line = CreateChangeLogLine();

            // act
            Func<ChangeLogLine> act = () => line.AssignToVersion(versionId, 0);

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}