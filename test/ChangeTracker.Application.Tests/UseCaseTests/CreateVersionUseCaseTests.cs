using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.CreateVersion;
using ChangeTracker.Domain;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests
{
    public class CreateVersionUseCaseTests
    {
        private readonly Mock<ICreateVersionOutputPort> _outputPortMock;
        private readonly ProjectDaoMock _projectDaoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoMock _versionDaoMock;

        public CreateVersionUseCaseTests()
        {
            _projectDaoMock = new ProjectDaoMock();
            _versionDaoMock = new VersionDaoMock();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<ICreateVersionOutputPort>();
        }

        [Fact]
        public async Task CreateVersion_Successful()
        {
            // arrange
            _projectDaoMock.Projects.Add(TestAccount.Project);
            var createVersionDto = new CreateVersionDto(TestAccount.Project.Id, "1.2.3");
            var createVersionUseCase =
                new CreateVersionUseCase(_versionDaoMock, _projectDaoMock, _unitOfWorkMock.Object);

            // act
            await createVersionUseCase.ExecuteAsync(_outputPortMock.Object, createVersionDto);

            // assert
            _outputPortMock.Verify(m => m.Created(It.Is<Guid>(x => x != Guid.Empty)));
        }

        [Fact]
        public async Task CreateVersion_VersionWithWhitespaceInTheMiddle_InvalidVersionFormatOutput()
        {
            // arrange
            _projectDaoMock.Projects.Add(TestAccount.Project);
            var createVersionDto = new CreateVersionDto(TestAccount.Project.Id, "1. .3");
            var createVersionUseCase =
                new CreateVersionUseCase(_versionDaoMock, _projectDaoMock, _unitOfWorkMock.Object);

            // act
            await createVersionUseCase.ExecuteAsync(_outputPortMock.Object, createVersionDto);

            // assert
            _outputPortMock.Verify(m => m.InvalidVersionFormat(It.Is<string>(x => x == "1. .3")));
        }

        [Fact]
        public async Task CreateVersion_NoProjectExists_ProjectDoesNotExistOutput()
        {
            // arrange
            var createVersionDto = new CreateVersionDto(TestAccount.Project.Id, "1.2.3");
            var createVersionUseCase =
                new CreateVersionUseCase(_versionDaoMock, _projectDaoMock, _unitOfWorkMock.Object);

            // act
            await createVersionUseCase.ExecuteAsync(_outputPortMock.Object, createVersionDto);

            // assert
            _outputPortMock.Verify(m => m.ProjectDoesNotExist());
        }

        [Fact]
        public async Task CreateVersion_WithInvalidVersionValue_VersionSchemeMismatchOutput()
        {
            // arrange
            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, DateTime.Parse("2021-04-04"), DateTime.Parse("2021-04-05")));

            var createVersionDto = new CreateVersionDto(TestAccount.Project.Id, "12*");
            var createVersionUseCase =
                new CreateVersionUseCase(_versionDaoMock, _projectDaoMock, _unitOfWorkMock.Object);

            // act
            await createVersionUseCase.ExecuteAsync(_outputPortMock.Object, createVersionDto);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotMatchScheme(It.Is<string>(x => x == "12*")));
        }
    }
}