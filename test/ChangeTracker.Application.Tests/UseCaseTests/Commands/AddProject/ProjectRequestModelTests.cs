using System;
using ChangeTracker.Application.UseCases.Command.AddProject;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AddProject
{
    public class ProjectRequestModelTests
    {
        private Guid _testAccountId;
        private string _testName;
        private Guid? _testSchemeId;

        public ProjectRequestModelTests()
        {
            _testAccountId = Guid.Parse("c1b588ee-069d-453e-8f74-fc43b7ae0649");
            _testName = "Project X";
            _testSchemeId = Guid.Parse("76a96500-6446-42b3-bb3d-5e318b338b0d");
        }

        private ProjectRequestModel CreateRequestModel() => new(_testAccountId, _testName, _testSchemeId);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.Name.Should().Be(_testName);
            requestModel.AccountId.Should().Be(_testAccountId);
            requestModel.VersioningSchemeId.HasValue.Should().BeTrue();
            requestModel.VersioningSchemeId.Should().HaveValue();
            requestModel.VersioningSchemeId!.Value.Should().Be(_testSchemeId!.Value);
        }

        [Fact]
        public void Create_WithEmptyAccountId_ArgumentException()
        {
            _testAccountId = Guid.Empty;

            Func<ProjectRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullName_ArgumentNullException()
        {
            _testName = null;

            Func<ProjectRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithEmptySchemeId_ArgumentNullException()
        {
            _testSchemeId = Guid.Empty;

            Func<ProjectRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}