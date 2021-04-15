using System;
using System.Collections.Generic;
using ChangeTracker.Application.UseCases.AddCompleteVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.AddCompleteVersion
{
    public class ChangeLogLineRequestModelTests
    {
        private List<string> _testIssues;
        private List<string> _testLabels;
        private string _testText;

        public ChangeLogLineRequestModelTests()
        {
            _testText = "some bug fixes";
            _testLabels = new List<string>(0);
            _testIssues = new List<string>(0);
        }

        private ChangeLogLineRequestModel CreateRequestModel() => new(_testText, _testLabels, _testIssues);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.Text.Should().Be(_testText);
            requestModel.Labels.Should().BeEmpty();
            requestModel.Issues.Should().BeEmpty();
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