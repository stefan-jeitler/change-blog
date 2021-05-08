using System;
using ChangeTracker.Application.UseCases.Commands.AssignAllPendingLinesToVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AssignAllPendingLinesToVersion
{
    public class VersionAssignmentRequestModelTests
    {
        private Guid _testProjectId;
        private string _testVersion;

        public VersionAssignmentRequestModelTests()
        {
            _testProjectId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
            _testVersion = "1.2.3";
        }

        private VersionAssignmentRequestModel CreateRequestModel() => new(_testProjectId, _testVersion);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.ProjectId.Should().Be(_testProjectId);
            requestModel.Version.Should().Be(_testVersion);
        }

        [Fact]
        public void Create_WithEmptyProjectId_ArgumentException()
        {
            _testProjectId = Guid.Empty;

            Func<VersionAssignmentRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersion_ArgumentNullException()
        {
            _testVersion = null;

            Func<VersionAssignmentRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}