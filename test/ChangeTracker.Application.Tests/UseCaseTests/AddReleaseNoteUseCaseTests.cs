using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AddChangeLogLine;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests
{
    public class AddChangeLogLineUseCaseTests
    {
        private readonly ChangeLogDaoMock _changeLogDaoMock;
        private readonly Mock<IAddChangeLogLineOutputPort> _outputPortMock;
        private readonly ProjectDaoMock _projectDaoMock;
        private readonly VersionDaoMock _versionDaoMock;

        public AddChangeLogLineUseCaseTests()
        {
            _projectDaoMock = new ProjectDaoMock();
            _versionDaoMock = new VersionDaoMock();
            _changeLogDaoMock = new ChangeLogDaoMock();
            _outputPortMock = new Mock<IAddChangeLogLineOutputPort>();
        }

        [Fact]
        public async Task AddChangeLogLine_NotExistingProject_ProjectDoesNotExistOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var addChangeLogLineDto =
                new AddChangeLogLineToVersionDto(TestAccount.Project.Id, "1.2", changeLogLine, labels);

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock);

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, addChangeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.ProjectDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_InvalidVersion_InvalidVersionFormatOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var changeLogLineDto =
                new AddChangeLogLineToVersionDto(TestAccount.Project.Id, "1. .3", changeLogLine, labels);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock);

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.InvalidVersionFormat(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_InvalidChangeLogLabel_InvalidLabelOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string> {"Bugfix", "ProxyIssue", "invalid label"};
            var changeLogLineDto =
                new AddChangeLogLineToVersionDto(TestAccount.Project.Id, "1.2", changeLogLine, labels);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _versionDaoMock.VersionInfo.Add(new ClVersionInfo(TestAccount.Project.Id, ClVersion.Parse("1.2"), null));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock);

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidLabels(
                    It.Is<List<string>>(x => x.Count == 1 && x.First() == "invalid label")), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_InvalidLineText_InvalidChangeLogLineOutput()
        {
            // arrange
            const string changeLogLine = "a";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var changeLogLineDto =
                new AddChangeLogLineToVersionDto(TestAccount.Project.Id, "1.2", changeLogLine, labels);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _versionDaoMock.VersionInfo.Add(new ClVersionInfo(TestAccount.Project.Id, ClVersion.Parse("1.2"), null));
            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock);

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidChangeLogLine(It.Is<string>(x => x == "a")), Times.Once);
        }
    }
}