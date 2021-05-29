using System;
using System.Collections.Generic;
using ChangeTracker.Application.UseCases.Commands.AddCompleteVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AddCompleteVersion
{
    public class CompleteVersionRequestModelTests
    {
        private static Guid _testUserId;
        private readonly string _testName;
        private List<ChangeLogLineRequestModel> _testLines;
        private Guid _testProductId;
        private bool _testReleaseImmediately;
        private string _testVersion;

        public CompleteVersionRequestModelTests()
        {
            _testProductId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
            _testUserId = Guid.Parse("294c4f04-85d4-4d5b-ae25-e6b618f1676f");
            _testVersion = "1.2.3";
            _testName = string.Empty;
            var testLine = new ChangeLogLineRequestModel("some text", new List<string>(0), new List<string>(0));
            _testLines = new List<ChangeLogLineRequestModel>(1) {testLine};
        }

        private CompleteVersionRequestModel CreateRequestModel() => new(_testUserId, _testProductId, _testVersion,
            _testName, _testLines, _testReleaseImmediately);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.Version.Should().Be(_testVersion);
            requestModel.ProductId.Should().Be(_testProductId);
            requestModel.Lines.Count.Should().Be(1);
            requestModel.ReleaseImmediately.Should().BeFalse();
        }

        [Fact]
        public void Create_WithEmptyProductId_ArgumentException()
        {
            _testProductId = Guid.Empty;

            Func<CompleteVersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersion_ArgumentNullException()
        {
            _testVersion = null;

            Func<CompleteVersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullLines_ArgumentNullException()
        {
            _testLines = null;

            Func<CompleteVersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_ReleaseImmediatelyIsTrue_ProperlySet()
        {
            _testReleaseImmediately = true;

            var requestModel = CreateRequestModel();

            requestModel.ReleaseImmediately.Should().BeTrue();
        }
    }
}