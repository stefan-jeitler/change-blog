using System;
using System.Collections.Generic;
using ChangeTracker.Application.UseCases.Commands.AddChangeLogLine.Models;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AddChangeLogLine
{
    public class VersionChangeLogLineRequestModelTests
    {
        private List<string> _testIssues;
        private List<string> _testLabels;
        private Guid _testProductId;
        private string _testText;
        private string _testVersion;

        public VersionChangeLogLineRequestModelTests()
        {
            _testProductId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
            _testVersion = "1.2.3";
            _testText = "some bug fixes";
            _testLabels = new List<string>(0);
            _testIssues = new List<string>(0);
        }

        private VersionChangeLogLineRequestModelRequestModel CreateRequestModel()
        {
            return new(_testProductId, _testVersion, _testText, _testLabels, _testIssues);
        }

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.ProductId.Should().Be(_testProductId);
            requestModel.Version.Should().Be(_testVersion);
            requestModel.Text.Should().Be(_testText);
            requestModel.Labels.Should().BeEmpty();
            requestModel.Issues.Should().BeEmpty();
        }

        [Fact]
        public void Create_WithEmptyProductId_ArgumentException()
        {
            _testProductId = Guid.Empty;

            Func<VersionChangeLogLineRequestModelRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersion_ArgumentNullException()
        {
            _testVersion = null;

            Func<VersionChangeLogLineRequestModelRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullText_ArgumentNullException()
        {
            _testText = null;

            Func<VersionChangeLogLineRequestModelRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullLabels_ArgumentNullException()
        {
            _testLabels = null;

            Func<VersionChangeLogLineRequestModelRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullIssues_ArgumentNullException()
        {
            _testIssues = null;

            Func<VersionChangeLogLineRequestModelRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}