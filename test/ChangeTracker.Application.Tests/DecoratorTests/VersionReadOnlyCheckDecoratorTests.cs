using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.Decorators;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace ChangeTracker.Application.Tests.DecoratorTests
{
    public class VersionReadOnlyCheckDecoratorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly IMemoryCache _memoryCache;
        private readonly VersionDaoStub _versionDaoStub;

        public VersionReadOnlyCheckDecoratorTests()
        {
            _changeLogDaoStub = new ChangeLogDaoStub();
            _versionDaoStub = new VersionDaoStub();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        [Fact]
        public async Task AddLine_RelatedVersionAlreadyReleased_Conflict()
        {
            // arrange
            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = new ClVersion(versionId, TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"),
                DateTime.Parse("2021-04-17"), DateTime.Parse("2021-04-17"), null);
            _versionDaoStub.Versions.Add(version);

            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var line = new ChangeLogLine(lineId, versionId, TestAccount.Project.Id, ChangeLogText.Parse("some text"),
                0U, DateTime.Parse("2021-04-17"));

            var decorator = new VersionReadonlyCheckDecorator(_changeLogDaoStub, _versionDaoStub, _memoryCache);

            // act
            var result = await decorator.AddLineAsync(line);

            // assert
            result.IsFailure.Should().BeTrue();
            result.Error.Reason.Should().StartWith("The related version has already been released.");
            _changeLogDaoStub.ChangeLogs.Should().BeEmpty();
        }

        [Fact]
        public async Task AddLine_RelatedVersionDeleted_Conflict()
        {
            // arrange
            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = new ClVersion(versionId, TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"),
                null, DateTime.Parse("2021-04-17"), DateTime.Parse("2021-04-17"));
            _versionDaoStub.Versions.Add(version);

            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var line = new ChangeLogLine(lineId, versionId, TestAccount.Project.Id, ChangeLogText.Parse("some text"),
                0U, DateTime.Parse("2021-04-17"));

            var decorator = new VersionReadonlyCheckDecorator(_changeLogDaoStub, _versionDaoStub, _memoryCache);

            // act
            var result = await decorator.AddLineAsync(line);

            // assert
            result.IsFailure.Should().BeTrue();
            result.Error.Reason.Should().StartWith("The related version has been deleted.");
            _changeLogDaoStub.ChangeLogs.Should().BeEmpty();
        }

        [Fact]
        public async Task AddLine_RelatedVersionIsNotReadOnly_SuccessfullyAdded()
        {
            // arrange
            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = new ClVersion(versionId, TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"),
                null, DateTime.Parse("2021-04-17"), null);
            _versionDaoStub.Versions.Add(version);

            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var line = new ChangeLogLine(lineId, versionId, TestAccount.Project.Id, ChangeLogText.Parse("some text"),
                0U, DateTime.Parse("2021-04-17"));

            var decorator = new VersionReadonlyCheckDecorator(_changeLogDaoStub, _versionDaoStub, _memoryCache);

            // act
            var result = await decorator.AddLineAsync(line);

            // assert
            result.IsSuccess.Should().BeTrue();
            _changeLogDaoStub.ChangeLogs.Should().Contain(x => x.Id == lineId);
        }

        [Fact]
        public async Task AddLine_ChangeLogLineIsPending_SuccessfullyAdded()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var line = new ChangeLogLine(lineId, null, TestAccount.Project.Id, ChangeLogText.Parse("some text"),
                0U, DateTime.Parse("2021-04-17"));

            var decorator = new VersionReadonlyCheckDecorator(_changeLogDaoStub, _versionDaoStub, _memoryCache);

            // act
            var result = await decorator.AddLineAsync(line);

            // assert
            result.IsSuccess.Should().BeTrue();
            _changeLogDaoStub.ChangeLogs.Should().Contain(x => x.Id == lineId);
        }

        [Fact]
        public async Task UpdateLine_ChangeLogLineIsPending_SuccessfullyAdded()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var line = new ChangeLogLine(lineId, null, TestAccount.Project.Id, ChangeLogText.Parse("some text"),
                0U, DateTime.Parse("2021-04-17"));

            var decorator = new VersionReadonlyCheckDecorator(_changeLogDaoStub, _versionDaoStub, _memoryCache);

            // act
            var result = await decorator.UpdateLineAsync(line);

            // assert
            result.IsSuccess.Should().BeTrue();
            _changeLogDaoStub.ChangeLogs.Should().Contain(x => x.Id == lineId);
        }

        [Fact]
        public async Task AddLines_ChangeLogLineIsPending_SuccessfullyAdded()
        {
            // arrange
            var firstLineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var firstLine = new ChangeLogLine(firstLineId, null, TestAccount.Project.Id,
                ChangeLogText.Parse("some text"),
                0U, DateTime.Parse("2021-04-17"));

            var secondLineId = Guid.Parse("4d755355-b527-4a4d-afbe-ab00a025609e");
            var secondLine = new ChangeLogLine(secondLineId, null, TestAccount.Project.Id,
                ChangeLogText.Parse("some other text"),
                0U, DateTime.Parse("2021-04-17"));

            var decorator = new VersionReadonlyCheckDecorator(_changeLogDaoStub, _versionDaoStub, _memoryCache);

            // act
            var result = await decorator.AddLinesAsync(new List<ChangeLogLine>(2) {firstLine, secondLine});

            // assert
            result.IsSuccess.Should().BeTrue();
            _changeLogDaoStub.ChangeLogs.Should().Contain(x => x.Id == firstLineId);
            _changeLogDaoStub.ChangeLogs.Should().Contain(x => x.Id == secondLineId);
            _changeLogDaoStub.ChangeLogs.Count.Should().Be(2);
        }

        [Fact]
        public async Task AddLines_RelatedVersionOfSecondLineIsReadOnly_SuccessfullyAdded()
        {
            // arrange
            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = new ClVersion(versionId, TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"),
                DateTime.Parse("2021-04-17"), DateTime.Parse("2021-04-17"), null);
            _versionDaoStub.Versions.Add(version);

            var firstLineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var firstLine = new ChangeLogLine(firstLineId, null, TestAccount.Project.Id,
                ChangeLogText.Parse("some text"),
                0U, DateTime.Parse("2021-04-17"));

            var secondLineId = Guid.Parse("4d755355-b527-4a4d-afbe-ab00a025609e");
            var secondLine = new ChangeLogLine(secondLineId, versionId, TestAccount.Project.Id,
                ChangeLogText.Parse("some other text"),
                0U, DateTime.Parse("2021-04-17"));

            var decorator = new VersionReadonlyCheckDecorator(_changeLogDaoStub, _versionDaoStub, _memoryCache);

            // act
            var result = await decorator.AddLinesAsync(new List<ChangeLogLine>(2) {firstLine, secondLine});

            // assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Reason.Should()
                .Be(
                    "The related version has already been released. ChangeLogLineId 4d755355-b527-4a4d-afbe-ab00a025609e");
            _changeLogDaoStub.ChangeLogs.Should().BeEmpty();
        }
    }
}