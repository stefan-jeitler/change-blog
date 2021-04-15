using System;
using System.Collections.Generic;
using ChangeTracker.Application.UseCases.AddCompleteVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.AddCompleteVersion
{
    public class CompleteVersionRequestModelTests
    {
        private List<ChangeLogLineRequestModel> _testLines;
        private Guid _testProjectId;
        private bool _testReleaseImmediately;
        private string _testVersion;

        public CompleteVersionRequestModelTests()
        {
            _testProjectId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
            _testVersion = "1.2.3";
            var testLine = new ChangeLogLineRequestModel("some text", new List<string>(0), new List<string>(0));
            _testLines = new List<ChangeLogLineRequestModel>(1) {testLine};
        }

        private CompleteVersionRequestModel CreateRequestModel() =>
            new(_testProjectId, _testVersion, _testLines, _testReleaseImmediately);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.Version.Should().Be(_testVersion);
            requestModel.ProjectId.Should().Be(_testProjectId);
            requestModel.Lines.Count.Should().Be(1);
            requestModel.ReleaseImmediately.Should().BeFalse();
        }

        [Fact]
        public void Create_WithEmptyProjectId_ArgumentException()
        {
            _testProjectId = Guid.Empty;

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