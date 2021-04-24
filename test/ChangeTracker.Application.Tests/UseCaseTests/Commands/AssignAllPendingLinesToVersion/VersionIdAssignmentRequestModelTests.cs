using System;
using ChangeTracker.Application.UseCases.Command.AssignAllPendingLinesToVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AssignAllPendingLinesToVersion
{
    public class VersionIdAssignmentRequestModelTests
    {
        private Guid _testProjectId;
        private Guid _testVersionId;

        public VersionIdAssignmentRequestModelTests()
        {
            _testProjectId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
            _testVersionId = Guid.Parse("30027f7d-91e4-4d08-afdc-a21d19656bb6");
        }

        public VersionIdAssignmentRequestModel CreateRequestModel() => new(_testProjectId, _testVersionId);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.ProjectId.Should().Be(_testProjectId);
            requestModel.VersionId.Should().Be(_testVersionId);
        }

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
    }
}