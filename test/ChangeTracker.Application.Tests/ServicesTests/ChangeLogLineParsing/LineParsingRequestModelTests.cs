using System;
using System.Collections.Generic;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.ServicesTests.ChangeLogLineParsing
{
    public class LineParsingRequestModelTests
    {
        private Guid _testProjectId;
        private Guid? _testVersionId;
        private string _testText;
        private IList<string> _testIssues;
        private IList<string> _testLabels;
        private uint? _testPosition;

        public LineParsingRequestModelTests()
        {
            _testProjectId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
            _testVersionId = Guid.Parse("30027f7d-91e4-4d08-afdc-a21d19656bb6");
            _testText = "some bug fixes";
            _testLabels = new List<string>(0);
            _testIssues = new List<string>(0);
            _testPosition = 0;
        }

        private LineParsingRequestModel CreateRequestModel() => new(_testProjectId, _testVersionId, _testText,
            _testLabels, _testIssues, _testPosition);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.ProjectId.Should().Be(_testProjectId);
            requestModel.VersionId.Should().Be(_testVersionId);
            requestModel.Text.Should().Be(_testText);
            requestModel.Labels.Should().BeEmpty();
            requestModel.Issues.Should().BeEmpty();
            requestModel.Position.Should().HaveValue();
        }

        [Fact]
        public void Create_WithEmptyProjectId_ArgumentException()
        {
            _testProjectId = Guid.Empty;

            Func<LineParsingRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyVersionId_ArgumentException()
        {
            _testVersionId = Guid.Empty;

            Func<LineParsingRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersionId_NoException()
        {
            _testVersionId = null;

            var requestModel = CreateRequestModel();

            requestModel.VersionId.Should().NotHaveValue();
        }

        [Fact]
        public void Create_NullText_ArgumentNullException()
        {
            _testText = null;

            Func<LineParsingRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_NullIssues_ArgumentNullException()
        {
            _testIssues = null;

            Func<LineParsingRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_NullLabels_ArgumentNullException()
        {
            _testLabels = null;

            Func<LineParsingRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_NullPosition_NoException()
        {
            _testPosition = null;

            var requestModel = CreateRequestModel();

            requestModel.Position.Should().NotHaveValue();
        }
    }
}