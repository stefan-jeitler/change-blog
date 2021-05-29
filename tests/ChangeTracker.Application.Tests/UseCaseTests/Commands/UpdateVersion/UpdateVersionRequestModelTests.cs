using System;
using ChangeTracker.Application.UseCases.Commands.SharedModels;
using ChangeTracker.Application.UseCases.Commands.UpdateVersion;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.UpdateVersion
{
    public class UpdateVersionRequestModelTests
    {
        private string _testName;
        private string _testVersion;
        private Guid _testUserId;
        private Guid _testProductId;

        public UpdateVersionRequestModelTests()
        {
            _testProductId = Guid.Parse("53161c63-e6c9-4908-8dac-9940896817c9");
            _testUserId = Guid.Parse("220cac7a-a4cd-41ed-9f3c-5118a97f75a2");
            _testName = "Release";
            _testVersion = "1.2.3";
        }

        private VersionRequestModel CreateRequestModel() => new(_testProductId, _testUserId, _testName, _testVersion);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var updateVersionRequestModel = CreateRequestModel();

            updateVersionRequestModel.ProductId.Should().Be(_testProductId);
            updateVersionRequestModel.Version.Should().Be(_testVersion);
            updateVersionRequestModel.Name.Should().Be(_testName);
        }

        [Fact]
        public void Create_WithEmptyProductId_ArgumentException()
        {
            _testProductId = Guid.Empty;

            Func<VersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyUserId_ArgumentException()
        {
            _testUserId = Guid.Empty;

            Func<VersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullName_NoException()
        {
            _testName = null;

            var requestModel = CreateRequestModel();

            requestModel.Name.Should().BeNull();
        }

        [Fact]
        public void Create_WithNullVersion_ArgumentException()
        {
            _testVersion = null;

            Func<VersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}