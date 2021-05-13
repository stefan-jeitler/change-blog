using System;
using System.Threading.Tasks;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.CloseProject;
using ChangeTracker.Domain;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.CloseProject
{
    public class CloseProjectInteractorTests
    {
        private readonly Mock<ICloseProjectOutputPort> _outputPortMock;
        private readonly ProjectDaoStub _projectDaoStub;

        public CloseProjectInteractorTests()
        {
            _projectDaoStub = new ProjectDaoStub();
            _outputPortMock = new Mock<ICloseProjectOutputPort>(MockBehavior.Strict);
        }

        private CloseProjectInteractor CreateInteractor() => new(_projectDaoStub);

        [Fact]
        public async Task CloseProject_ProjectDoesNotExist_ProjectDoesNotExistOutput()
        {
            // arrange
            var notExistingProjectId = Guid.Parse("658ab2ec-ac88-47aa-af14-2093a0d07f4f");
            _outputPortMock.Setup(m => m.ProjectDoesNotExist());
            var interactor = CreateInteractor();

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, notExistingProjectId);

            // assert
            _outputPortMock.Verify(m => m.ProjectDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task CloseProject_ProjectAlreadyClosed_ProjectAlreadyClosedOutput()
        {
            // arrange
            var project = new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, TestAccount.Project.CreatedAt, DateTime.Parse("2021-05-13"));
            _projectDaoStub.Projects.Add(project);
            _outputPortMock.Setup(m => m.ProjectAlreadyClosed());
            var interactor = CreateInteractor();

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, project.Id);

            // assert
            _outputPortMock.Verify(m => m.ProjectAlreadyClosed(), Times.Once);
        }

        [Fact]
        public async Task CloseProject_ConflictWhileDeleting_ConflictOutput()
        {
            // arrange
            var project = new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, TestAccount.Project.CreatedAt, null);
            _projectDaoStub.Projects.Add(project);
            _projectDaoStub.ProduceConflict = true;

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));
            var interactor = CreateInteractor();

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, project.Id);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CloseProject_HappyPath_ProjectClosedOutput()
        {
            // arrange
            var project = new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, TestAccount.Project.CreatedAt, null);
            _projectDaoStub.Projects.Add(project);

            _outputPortMock.Setup(m => m.ProjectClosed(It.IsAny<Guid>()));
            var interactor = CreateInteractor();

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, project.Id);

            // assert
            _outputPortMock.Verify(m => m.ProjectClosed(It.IsAny<Guid>()), Times.Once);
        }
    }
}