using System;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

// ReSharper disable InconsistentNaming

namespace ChangeTracker.Domain.Tests.ChangeLogTests
{
    public class ChangeLogLineTests
    {
        private const uint TestPosition = 5;
        private static readonly Guid TestId = Guid.Parse("51d89265-52c2-4a38-a0fe-b99bdc5523d0");
        private static readonly Guid TestVersionId = Guid.Parse("66845d0a-45bc-4834-96d0-b48c2c403628");
        private static readonly Guid TestProjectId = Guid.Parse("ef5656e5-15f0-418d-b3a4-b69f1c3abac5");
        private static readonly ChangeLogText TestText = ChangeLogText.Parse("New feature added");
        private static readonly DateTime TestCreationDate = DateTime.Parse("2021-04.02T18:28");
        private static readonly DateTime TestDeletionDate = DateTime.Parse("2021-04.02T18:28");

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var line = new ChangeLogLine(TestId,
                TestVersionId,
                TestProjectId,
                TestText,
                TestPosition,
                TestCreationDate,
                null,
                null);

            line.Id.Should().Be(TestId);
            line.VersionId.Should().Be(TestVersionId);
            line.ProjectId.Should().Be(TestProjectId);
            line.Text.Should().Be(TestText);
            line.Position.Should().Be(TestPosition);
            line.CreatedAt.Should().Be(TestCreationDate);
            line.Labels.Should().BeEmpty();
            line.DeletedAt.Should().BeNull();
        }

        [Fact]
        public void Create_WithOverloadedConstructor_IdIsGenerated()
        {
            var line = new ChangeLogLine(TestVersionId, TestProjectId, TestText, TestPosition);

            line.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void Create_WithDeletionDate_DateIsProperlySet()
        {
            var line = new ChangeLogLine(TestId,
                null,
                TestProjectId,
                TestText,
                TestPosition,
                TestCreationDate,
                null,
                null,
                TestDeletionDate);

            line.DeletedAt.HasValue.Should().BeTrue();
            line.DeletedAt.Value.Should().Be(TestDeletionDate);
        }

        [Fact]
        public void Create_WithOverloadedConstructor_IdAndCreatedAtDatetimeAreGenerated()
        {
            var line = new ChangeLogLine(TestVersionId, TestProjectId, TestText, TestPosition);

            line.Id.Should().NotBe(Guid.Empty);
            line.CreatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
            line.CreatedAt.Should().BeBefore(DateTime.UtcNow);
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            Func<ChangeLogLine> act = () => new ChangeLogLine(Guid.Empty,
                TestVersionId,
                TestProjectId,
                TestText,
                TestPosition,
                TestCreationDate);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyVersionId_Successful()
        {
            Func<ChangeLogLine> act = () => new ChangeLogLine(TestId,
                Guid.Empty,
                TestProjectId,
                TestText,
                TestPosition,
                TestCreationDate);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersionId_IsPendingChangeLogLine()
        {
            var line = new ChangeLogLine(TestId,
                null,
                TestProjectId,
                TestText,
                TestPosition,
                TestCreationDate);

            line.IsPending.Should().BeTrue();
        }

        [Fact]
        public void Create_WithVersionIdAndDeletionDate_InvalidOperationException()
        {
            Func<ChangeLogLine> act = () => new ChangeLogLine(TestId,
                TestVersionId,
                TestProjectId,
                TestText,
                TestPosition,
                TestCreationDate,
                TestDeletionDate);

            act.Should().ThrowExactly<InvalidOperationException>("Released changeLog lines cannot be deleted.");
        }

        [Fact]
        public void Create_WithEmptyProjectId_ArgumentException()
        {
            Func<ChangeLogLine> act = () => new ChangeLogLine(TestId,
                TestVersionId,
                Guid.Empty,
                TestText,
                TestPosition,
                TestCreationDate);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullText_ArgumentNullException()
        {
            Func<ChangeLogLine> act = () => new ChangeLogLine(TestId,
                TestVersionId,
                TestProjectId,
                null,
                TestPosition,
                TestCreationDate);

            act.Should().ThrowExactly<ArgumentNullException>();
        }
        
        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidCreatedAtDate_ArgumentNullException(string invalidDate)
        {
            var createdAt = DateTime.Parse(invalidDate);

            Func<ChangeLogLine> act = () =>
                new ChangeLogLine(TestId, TestVersionId, TestProjectId, TestText, TestPosition, createdAt);

            act.Should().ThrowExactly<ArgumentException>();
        }


        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidDeletedAtDate_ArgumentNullException(string invalidDate)
        {
            var deletedAt = DateTime.Parse(invalidDate);

            Func<ChangeLogLine> act = () =>
                new ChangeLogLine(TestId, TestVersionId, TestProjectId, TestText, TestPosition, TestCreationDate,
                    deletedAt);

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}