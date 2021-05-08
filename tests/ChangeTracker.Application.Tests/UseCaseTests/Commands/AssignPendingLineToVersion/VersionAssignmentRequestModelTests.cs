using System;
using ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AssignPendingLineToVersion
{
    public class VersionAssignmentRequestModelTests
    {
        private Guid _testLineId;
        private Guid _testProjectId;
        private string _testVersion;

        public VersionAssignmentRequestModelTests()
        {
            _testProjectId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
            _testVersion = "1.2.3";
            _testLineId = Guid.Parse("1763b2e7-9835-4992-8f73-8c2026530b2c");
        }

        private VersionAssignmentRequestModel CreateRequestModel() => new(_testProjectId, _testVersion, _testLineId);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.ProjectId.Should().Be(_testProjectId);
            requestModel.Version.Should().Be(_testVersion);
            requestModel.ChangeLogLineId.Should().Be(_testLineId);
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

        [Fact]
        public void Create_WithEmptyLineId_ArgumentException()
        {
            _testLineId = Guid.Empty;

            Func<VersionAssignmentRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}