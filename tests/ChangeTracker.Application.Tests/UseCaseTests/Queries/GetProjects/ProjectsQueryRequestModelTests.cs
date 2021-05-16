using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Queries.GetProjects;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Queries.GetProjects
{
    public class ProjectsQueryRequestModelTests
    {

        private Guid _testUserId;
        private Guid _testAccountId;
        private readonly Guid? _testLastProjectId;
        private ushort _testCount;


        public ProjectsQueryRequestModelTests()
        {
            _testUserId = TestAccount.UserId;
            _testAccountId = TestAccount.Id;
            _testLastProjectId = Guid.Parse("33f9a7a4-5a0b-4ac8-b074-d97e13e8596c");
            _testCount = 100;
        }

        private ProjectsQueryRequestModel CreateRequestModel() =>
            new(_testUserId, _testAccountId, _testLastProjectId, _testCount, true); 

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.UserId.Should().Be(_testUserId);
            requestModel.AccountId.Should().Be(_testAccountId);
            requestModel.LastProjectId.Should().Be(_testLastProjectId);
            requestModel.Count.Should().Be(_testCount);
            requestModel.IncludeClosedProjects.Should().Be(true);
        }

        [Fact]
        public void Create_WithEmptyUserId_ArgumentException()
        {
            // arrange
            _testUserId = Guid.Empty;

            // act
            Func<ProjectsQueryRequestModel> act = CreateRequestModel;

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyAccountId_ArgumentException()
        {
            // arrange
            _testAccountId = Guid.Empty;

            // act
            Func<ProjectsQueryRequestModel> act = CreateRequestModel;

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithTooHighCountValue_ValueCapped()
        {
            // arrange
            _testCount = 500;

            // act
            var requestModel = CreateRequestModel();

            // assert
            requestModel.Count.Should().Be(ProjectsQueryRequestModel.MaxChunkCount);
        }
    }
}
