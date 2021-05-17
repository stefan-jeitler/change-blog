using System;
using System.Collections.Generic;
using ChangeTracker.Application.UseCases.Commands.UpdateChangeLogLine;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.UpdateChangeLogLine
{
    public class ChangeLogLineRequestModelTests
    {
        private IList<string> _testIssues;
        private IList<string> _testLabels;
        private Guid _testLineId;
        private string _testText;

        public ChangeLogLineRequestModelTests()
        {
            _testLineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            _testText = "some features added";
            _testLabels = Array.Empty<string>();
            _testIssues = Array.Empty<string>();
        }

        private ChangeLogLineRequestModel CreateRequestModel()
        {
            return new(_testLineId, _testText, _testLabels, _testIssues);
        }

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.ChangeLogLineId.Should().Be(_testLineId);
            requestModel.Text.Should().Be(_testText);
            requestModel.Labels.Should().BeEmpty();
            requestModel.Issues.Should().BeEmpty();
        }

        [Fact]
        public void Create_WithEmptyLineId_ArgumentException()
        {
            _testLineId = Guid.Empty;

            Func<ChangeLogLineRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullText_ArgumentNullException()
        {
            _testText = null;

            Func<ChangeLogLineRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullLabels_ArgumentNullException()
        {
            _testLabels = null;

            Func<ChangeLogLineRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullIssues_ArgumentNullException()
        {
            _testIssues = null;

            Func<ChangeLogLineRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}