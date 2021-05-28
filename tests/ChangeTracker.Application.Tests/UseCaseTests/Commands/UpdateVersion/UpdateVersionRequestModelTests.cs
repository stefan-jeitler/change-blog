using System;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.UpdateVersion;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.UpdateVersion
{
    public class UpdateVersionRequestModelTests
    {

        private Guid _testVersionId;
        private string _testName;
        private string _testVersion;

        public UpdateVersionRequestModelTests()
        {
            _testVersionId = Guid.Parse("53161c63-e6c9-4908-8dac-9940896817c9");
            _testName = "Release";
            _testVersion = "1.2.3";
        }

        private UpdateVersionRequestModel CreateRequestModel() => new(_testVersionId, _testName, _testVersion);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var updateVersionRequestModel = CreateRequestModel();

            updateVersionRequestModel.VersionId.Should().Be(_testVersionId);
            updateVersionRequestModel.Version.Should().Be(_testVersion);
            updateVersionRequestModel.Name.Should().Be(_testName);
        }

        [Fact]
        public void Create_WithEmptyVersionId_ArgumentException()
        {
            _testVersionId = Guid.Empty;

            Func<UpdateVersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullName_ArgumentException()
        {
            _testName = null;

            Func<UpdateVersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullVersion_ArgumentException()
        {
            _testVersion = null;

            Func<UpdateVersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}