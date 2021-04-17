using System;
using ChangeTracker.Application.UseCases.AssignPendingLineToVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.AssignPendingLineToVersion
{
    public class VersionIdAssignmentRequestModelTests
    {
        private Guid _testLineId;
        private Guid _testProjectId;
        private Guid _testVersionId;

        public VersionIdAssignmentRequestModelTests()
        {
            _testProjectId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
            _testVersionId = Guid.Parse("30027f7d-91e4-4d08-afdc-a21d19656bb6");
            _testLineId = Guid.Parse("1763b2e7-9835-4992-8f73-8c2026530b2c");
        }

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.ProjectId.Should().Be(_testProjectId);
            requestModel.VersionId.Should().Be(_testVersionId);
            requestModel.ChangeLogLineId.Should().Be(_testLineId);
        }

        public VersionIdAssignmentRequestModel CreateRequestModel() => new(_testProjectId, _testVersionId, _testLineId);

        [Fact]
        public void Create_WithEmptyProjectId_ArgumentException()
        {
            _testProjectId = Guid.Empty;

            Func<VersionIdAssignmentRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyVersionId_ArgumentException()
        {
            _testVersionId = Guid.Empty;

            Func<VersionIdAssignmentRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyLineId_ArgumentException()
        {
            _testLineId = Guid.Empty;

            Func<VersionIdAssignmentRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}