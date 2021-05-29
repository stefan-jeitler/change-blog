using System;
using ChangeTracker.Application.UseCases.Commands.AddVersion;
using ChangeTracker.Application.UseCases.Commands.SharedModels;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AddVersion
{
    public class VersionRequestModelTests
    {
        private static Guid _testUserId;
        private readonly string _testName;
        private Guid _testProductId;
        private string _testVersion;

        public VersionRequestModelTests()
        {
            _testProductId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
            _testUserId = Guid.Parse("294c4f04-85d4-4d5b-ae25-e6b618f1676f");
            _testName = "";
            _testVersion = "1.2.3";
        }

        private VersionRequestModel CreateRequestModel() => new(_testUserId, _testProductId, _testVersion, _testName);

        [Fact]
        public void Create_WithEmptyProductId_ArgumentException()
        {
            _testProductId = Guid.Empty;

            Func<VersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersion_ArgumentNullException()
        {
            _testVersion = null;

            Func<VersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}