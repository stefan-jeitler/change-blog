using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.CreateCompleteVersion;
using ChangeTracker.Application.UseCases.CreateCompleteVersion.DTOs;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests
{
    public class CreateCompleteVersionUseCaseTests
    {
        private readonly ChangeLogDaoMock _changeLogDaoMock;
        private readonly Mock<ICreateCompleteVersionOutputPort> _outputPortMock;
        private readonly ProjectDaoMock _projectDaoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoMock _versionDaoMock;

        public CreateCompleteVersionUseCaseTests()
        {
            _projectDaoMock = new ProjectDaoMock();
            _versionDaoMock = new VersionDaoMock();
            _changeLogDaoMock = new ChangeLogDaoMock();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<ICreateCompleteVersionOutputPort>(MockBehavior.Strict);
        }

        [Fact]
        public async Task ReleaseNewVersion_ValidVersion_Successful()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineDto>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var newVersionDto = new CompleteVersionDto(TestAccount.Project.Id, "1.23", changeLogLines);

            _projectDaoMock.Projects.Add(TestAccount.Project);

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            var releaseNewVersionUseCase = new CreateCompleteVersionUseCase(_projectDaoMock, _versionDaoMock,
                _changeLogDaoMock, _unitOfWorkMock.Object);

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, newVersionDto);

            // assert
            _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
            var version = _versionDaoMock.Versions.Single(x => x.ProjectId == TestAccount.Project.Id);
            version.Value.Should().Be(ClVersionValue.Parse("1.23"));
            version.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task ReleaseNewVersion_NotExistingProject_ProjectDoesNotExistOutput()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineDto>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var newVersionDto = new CompleteVersionDto(TestAccount.Project.Id, "1.23", changeLogLines);


            _outputPortMock.Setup(m => m.ProjectDoesNotExist());
            var releaseNewVersionUseCase = new CreateCompleteVersionUseCase(_projectDaoMock, _versionDaoMock,
                _changeLogDaoMock, _unitOfWorkMock.Object);

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, newVersionDto);

            // assert
            _outputPortMock.Verify(m => m.ProjectDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task ReleaseNewVersion_InvalidVersionFormat_InvalidVersionFormatOutput()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineDto>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var newVersionDto = new CompleteVersionDto(TestAccount.Project.Id, "1. .2", changeLogLines);

            _projectDaoMock.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.InvalidVersionFormat(It.IsAny<string>()));
            var releaseNewVersionUseCase = new CreateCompleteVersionUseCase(_projectDaoMock, _versionDaoMock,
                _changeLogDaoMock, _unitOfWorkMock.Object);

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, newVersionDto);

            // assert
            _outputPortMock.Verify(m => 
                    m.InvalidVersionFormat(It.Is<string>(x => x == "1. .2")), Times.Once);
        }

        [Fact]
        public async Task ReleaseNewVersion_VersionDoesNotMachScheme_InvalidVersionFormatOutput()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineDto>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var newVersionDto = new CompleteVersionDto(TestAccount.Project.Id, "*.23", changeLogLines);

            _projectDaoMock.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.VersionDoesNotMatchScheme(It.IsAny<string>()));
            var releaseNewVersionUseCase = new CreateCompleteVersionUseCase(_projectDaoMock, _versionDaoMock,
                _changeLogDaoMock, _unitOfWorkMock.Object);

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, newVersionDto);

            // assert
            _outputPortMock.Verify(m => 
                    m.VersionDoesNotMatchScheme(It.Is<string>(x => x == "*.23")), Times.Once);
        }

        [Fact]
        public async Task ReleaseNewVersion_LineDuplicates_DuplicatesRemoved()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineDto>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var newVersionDto = new CompleteVersionDto(TestAccount.Project.Id, "1.23", changeLogLines);

            _projectDaoMock.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            var releaseNewVersionUseCase = new CreateCompleteVersionUseCase(_projectDaoMock, _versionDaoMock,
                _changeLogDaoMock, _unitOfWorkMock.Object);

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, newVersionDto);

            // assert
            _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
            var createdLines = _changeLogDaoMock.ChangeLogs
                .Where(x => x.ProjectId == TestAccount.Project.Id)
                .ToList();

            createdLines.Count.Should().Be(3);
        }

        [Fact]
        public async Task CreateCompleteVersion_ReleaseImmediately_ReleaseDateIsNotNull()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineDto>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var newVersionDto = new CompleteVersionDto(TestAccount.Project.Id, "1.23", changeLogLines, true);

            _projectDaoMock.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            var releaseNewVersionUseCase = new CreateCompleteVersionUseCase(_projectDaoMock, _versionDaoMock,
                _changeLogDaoMock, _unitOfWorkMock.Object);

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, newVersionDto);

            // assert
            _versionDaoMock.Versions.Single().ReleasedAt.HasValue.Should().BeTrue();
        }

        [Fact]
        public async Task CreateCompleteVersion_DoNotReleaseVersionYet_ReleaseDateIsNull()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineDto>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var newVersionDto = new CompleteVersionDto(TestAccount.Project.Id, "1.23", changeLogLines, false);

            _projectDaoMock.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            var releaseNewVersionUseCase = new CreateCompleteVersionUseCase(_projectDaoMock, _versionDaoMock,
                _changeLogDaoMock, _unitOfWorkMock.Object);

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, newVersionDto);

            // assert
            _versionDaoMock.Versions.Single().ReleasedAt.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task ReleaseNewVersion_TooManyChangeLogLines_MaxChangeLogLinesReached()
        {
            // arrange
            var changeLogLines = Enumerable.Range(0, 101)
                .Select(x => new ChangeLogLineDto($"{x:D5}", new List<string> {"Security"}, new List<string>()))
                .ToList();

            var newVersionDto = new CompleteVersionDto(TestAccount.Project.Id, "1.23", changeLogLines);

            _projectDaoMock.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.MaxChangeLogLinesReached(It.IsAny<int>()));
            var releaseNewVersionUseCase = new CreateCompleteVersionUseCase(_projectDaoMock, _versionDaoMock,
                _changeLogDaoMock, _unitOfWorkMock.Object);

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, newVersionDto);

            // assert
            _outputPortMock.Verify(m => 
                m.MaxChangeLogLinesReached(It.Is<int>(x => x == ChangeLogsMetadata.MaxChangeLogLines)), Times.Once);
        }

        [Fact]
        public async Task ReleaseNewVersion_ValidVersion_PositionsProperlySet()
        {
            // arrange
            var changeLogLines = Enumerable.Range(0, 50)
                .Select(x => new ChangeLogLineDto($"{x:D5}", new List<string> {"Security"}, new List<string>()))
                .ToList();

            var newVersionDto = new CompleteVersionDto(TestAccount.Project.Id, "1.23", changeLogLines);

            _projectDaoMock.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            var releaseNewVersionUseCase = new CreateCompleteVersionUseCase(_projectDaoMock, _versionDaoMock,
                _changeLogDaoMock, _unitOfWorkMock.Object);

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, newVersionDto);

            // assert
            foreach (var (lineDto, i) in changeLogLines.Select((x, i) => (x, i)))
            {
                _changeLogDaoMock.ChangeLogs[i].Position.Should().Be(uint.Parse(lineDto.Text));
            }
        }

        [Fact]
        public async Task ReleaseNewVersion_ValidVersion_TransactionStartedAndCommitted()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineDto>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var newVersionDto = new CompleteVersionDto(TestAccount.Project.Id, "1.23", changeLogLines);

            _projectDaoMock.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            var releaseNewVersionUseCase = new CreateCompleteVersionUseCase(_projectDaoMock, _versionDaoMock,
                _changeLogDaoMock, _unitOfWorkMock.Object);

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, newVersionDto);

            // assert
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }
    }
}