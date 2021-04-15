using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AddCompleteVersion;
using ChangeTracker.Application.UseCases.AddCompleteVersion.Models;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.AddCompleteVersion
{
    public class AddCompleteVersionInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IAddCompleteVersionOutputPort> _outputPortMock;
        private readonly ProjectDaoStub _projectDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;

        public AddCompleteVersionInteractorTests()
        {
            _projectDaoStub = new ProjectDaoStub();
            _versionDaoStub = new VersionDaoStub();
            _changeLogDaoStub = new ChangeLogDaoStub();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<IAddCompleteVersionOutputPort>(MockBehavior.Strict);
        }

        [Fact]
        public async Task ReleaseNewVersion_ValidVersion_Successful()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineRequestModel>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var versionRequestModel = new CompleteVersionRequestModel(TestAccount.Project.Id, "1.23", changeLogLines);

            _projectDaoStub.Projects.Add(TestAccount.Project);

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            var releaseNewVersionUseCase = new AddCompleteVersionInteractor(_projectDaoStub, _versionDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object, new ChangeLogLineParsingService(_changeLogDaoStub));

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
            var version = _versionDaoStub.Versions.Single(x => x.ProjectId == TestAccount.Project.Id);
            version.Value.Should().Be(ClVersionValue.Parse("1.23"));
            version.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task ReleaseNewVersion_NotExistingProject_ProjectDoesNotExistOutput()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineRequestModel>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var versionRequestModel = new CompleteVersionRequestModel(TestAccount.Project.Id, "1.23", changeLogLines);


            _outputPortMock.Setup(m => m.ProjectDoesNotExist());
            var releaseNewVersionUseCase = new AddCompleteVersionInteractor(_projectDaoStub, _versionDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object, new ChangeLogLineParsingService(_changeLogDaoStub));

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.ProjectDoesNotExist(), Times.Once);
        }


        [Fact]
        public async Task ReleaseNewVersion_VersionExists_VersionAlreadyExistsOutput()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineRequestModel>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"})
            };
            var versionRequestModel = new CompleteVersionRequestModel(TestAccount.Project.Id, "1.23", changeLogLines);

            _projectDaoStub.Projects.Add(TestAccount.Project);
            _versionDaoStub.Versions.Add(new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.23")));

            _outputPortMock.Setup(m => m.VersionAlreadyExists(It.IsAny<string>()));
            var releaseNewVersionUseCase = new AddCompleteVersionInteractor(_projectDaoStub, _versionDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object, new ChangeLogLineParsingService(_changeLogDaoStub));

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyExists(It.Is<string>(x => x == "1.23")), Times.Once);
        }

        [Fact]
        public async Task ReleaseNewVersion_InvalidVersionFormat_InvalidVersionFormatOutput()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineRequestModel>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var versionRequestModel = new CompleteVersionRequestModel(TestAccount.Project.Id, "1. .2", changeLogLines);

            _projectDaoStub.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.InvalidVersionFormat(It.IsAny<string>()));
            var releaseNewVersionUseCase = new AddCompleteVersionInteractor(_projectDaoStub, _versionDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object, new ChangeLogLineParsingService(_changeLogDaoStub));

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m =>
                m.InvalidVersionFormat(It.Is<string>(x => x == "1. .2")), Times.Once);
        }

        [Fact]
        public async Task ReleaseNewVersion_VersionDoesNotMachScheme_InvalidVersionFormatOutput()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineRequestModel>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var versionRequestModel = new CompleteVersionRequestModel(TestAccount.Project.Id, "*.23", changeLogLines);

            _projectDaoStub.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.VersionDoesNotMatchScheme(It.IsAny<string>()));
            var releaseNewVersionUseCase = new AddCompleteVersionInteractor(_projectDaoStub, _versionDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object, new ChangeLogLineParsingService(_changeLogDaoStub));

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m =>
                m.VersionDoesNotMatchScheme(It.Is<string>(x => x == "*.23")), Times.Once);
        }

        [Fact]
        public async Task ReleaseNewVersion_LineDuplicates_DuplicatesRemoved()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineRequestModel>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var versionRequestModel = new CompleteVersionRequestModel(TestAccount.Project.Id, "1.23", changeLogLines);

            _projectDaoStub.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            var releaseNewVersionUseCase = new AddCompleteVersionInteractor(_projectDaoStub, _versionDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object, new ChangeLogLineParsingService(_changeLogDaoStub));

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
            var createdLines = _changeLogDaoStub.ChangeLogs
                .Where(x => x.ProjectId == TestAccount.Project.Id)
                .ToList();

            createdLines.Count.Should().Be(3);
        }

        [Fact]
        public async Task ReleaseNewVersion_TooManyChangeLogLines_MaxChangeLogLinesReached()
        {
            // arrange
            var changeLogLines = Enumerable.Range(0, 101)
                .Select(x => new ChangeLogLineRequestModel($"{x:D5}", new List<string> { "Security" }, new List<string>()))
                .ToList();

            var versionRequestModel = new CompleteVersionRequestModel(TestAccount.Project.Id, "1.23", changeLogLines);

            _projectDaoStub.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.TooManyLines(It.IsAny<int>()));
            var releaseNewVersionUseCase = new AddCompleteVersionInteractor(_projectDaoStub, _versionDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object, new ChangeLogLineParsingService(_changeLogDaoStub));

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m =>
                m.TooManyLines(It.Is<int>(x => x == ChangeLogsMetadata.MaxChangeLogLines)), Times.Once);
        }

        [Fact]
        public async Task CreateCompleteVersion_ReleaseImmediately_ReleaseDateIsNotNull()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineRequestModel>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var versionRequestModel = new CompleteVersionRequestModel(TestAccount.Project.Id, "1.23", changeLogLines, true);

            _projectDaoStub.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            var releaseNewVersionUseCase = new AddCompleteVersionInteractor(_projectDaoStub, _versionDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object, new ChangeLogLineParsingService(_changeLogDaoStub));

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _versionDaoStub.Versions.Single().ReleasedAt.HasValue.Should().BeTrue();
        }

        [Fact]
        public async Task CreateCompleteVersion_DoNotReleaseVersionYet_ReleaseDateIsNull()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineRequestModel>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var versionRequestModel = new CompleteVersionRequestModel(TestAccount.Project.Id, "1.23", changeLogLines);

            _projectDaoStub.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            var releaseNewVersionUseCase = new AddCompleteVersionInteractor(_projectDaoStub, _versionDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object, new ChangeLogLineParsingService(_changeLogDaoStub));

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _versionDaoStub.Versions.Single().ReleasedAt.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task ReleaseNewVersion_ValidVersion_PositionsProperlySet()
        {
            // arrange
            var changeLogLines = Enumerable.Range(0, 50)
                .Select(x => new ChangeLogLineRequestModel($"{x:D5}", new List<string> {"Security"}, new List<string>()))
                .ToList();

            var versionRequestModel = new CompleteVersionRequestModel(TestAccount.Project.Id, "1.23", changeLogLines);

            _projectDaoStub.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            var releaseNewVersionUseCase = new AddCompleteVersionInteractor(_projectDaoStub, _versionDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object, new ChangeLogLineParsingService(_changeLogDaoStub));

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            foreach (var (lineRequestModel, i) in changeLogLines.Select((x, i) => (x, i)))
            {
                _changeLogDaoStub.ChangeLogs[i].Position.Should().Be(uint.Parse(lineRequestModel.Text));
            }
        }

        [Fact]
        public async Task ReleaseNewVersion_ValidVersion_TransactionStartedAndCommitted()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineRequestModel>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var versionRequestModel = new CompleteVersionRequestModel(TestAccount.Project.Id, "1.23", changeLogLines);

            _projectDaoStub.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            var releaseNewVersionUseCase = new AddCompleteVersionInteractor(_projectDaoStub, _versionDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object, new ChangeLogLineParsingService(_changeLogDaoStub));

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }

        [Fact]
        public async Task ReleaseNewVersion_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            var changeLogLines = new List<ChangeLogLineRequestModel>
            {
                new("Proxy bug resolved", new List<string> {"ProxyStrikesBack"}, new List<string> {"#123"}),
                new("New feature added", new List<string> {"Feature"}, new List<string>()),
                new("Allow https only", new List<string> {"Security"}, new List<string>())
            };
            var versionRequestModel = new CompleteVersionRequestModel(TestAccount.Project.Id, "1.23", changeLogLines);

            _changeLogDaoStub.ProduceConflict = true;
            _projectDaoStub.Projects.Add(TestAccount.Project);
            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));
            _unitOfWorkMock.Setup(m => m.Start());

            var releaseNewVersionUseCase = new AddCompleteVersionInteractor(_projectDaoStub, _versionDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object, new ChangeLogLineParsingService(_changeLogDaoStub));

            // act
            await releaseNewVersionUseCase.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }
    }
}