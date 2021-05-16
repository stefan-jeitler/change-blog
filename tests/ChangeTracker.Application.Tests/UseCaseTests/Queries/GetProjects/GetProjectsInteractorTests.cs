using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Queries.GetProjects;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Queries.GetProjects
{
    public class GetProjectsInteractorTests
    {

        private readonly ProjectDaoStub _projectDaoStub;
        private readonly UserDaoStub _userDaoStub;

        public GetProjectsInteractorTests()
        {
            _projectDaoStub = new ProjectDaoStub();
            _userDaoStub = new UserDaoStub();
        }

        private GetProjectsInteractor CreateInteractor() => new(_projectDaoStub, _userDaoStub);

        [Fact]
        public async Task GetProjects_HappyPath_Successful()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);
            _projectDaoStub.Projects.Add(TestAccount.Project);
            var interactor = CreateInteractor();
            var requestModel = new ProjectsQueryRequestModel(TestAccount.UserId, TestAccount.Id, null, 1, true);

            // act
            var projects = await interactor.ExecuteAsync(requestModel);

            // assert
            projects.Should().HaveCount(1);
            projects.Should().ContainSingle(x => x.Id == TestAccount.Project.Id);
        }

        [Fact]
        public async Task GetProjects_NotExistingAccountId_Successful()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);
            _projectDaoStub.Projects.Add(TestAccount.Project);
            var interactor = CreateInteractor();
            var notExistingAccountId = Guid.Parse("3639c610-bd58-4924-a5fa-ec19b3a324b0");
            var requestModel = new ProjectsQueryRequestModel(TestAccount.UserId, notExistingAccountId, null, 1, true);

            // act
            var projects = await interactor.ExecuteAsync(requestModel);

            // assert
            projects.Should().BeEmpty();
        }

        [Fact]
        public async Task GetProject_HappyPath_Successful()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);
            _projectDaoStub.Projects.Add(TestAccount.Project);
            var interactor = CreateInteractor();

            // act
            var project = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Project.Id);

            // assert
            project.HasValue.Should().BeTrue();
        }

        [Fact]
        public async Task GetProject_CreateAt_ProperlyConvertedToUserTimeZone()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);
            _projectDaoStub.Projects.Add(TestAccount.Project);
            var interactor = CreateInteractor();
            var createdAtLocal = TestAccount.Project.CreatedAt.ToLocal(TestAccount.User.TimeZone);

            // act
            var project = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Project.Id);

            // assert
            project.Value.CreatedAt.Should().Be(createdAtLocal);
        }
    }
}
