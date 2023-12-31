using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Conflicts;
using ChangeBlog.Application.Proxies;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Domain;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace ChangeBlog.Application.Tests.ProxyTests;

public class ReadOnlyCheckProxyTests
{
    private readonly FakeChangeLogDao _fakeChangeLogDao;
    private readonly FakeProductDao _fakeProductDao;
    private readonly FakeVersionDao _fakeVersionDao;
    private readonly IMemoryCache _memoryCache;

    public ReadOnlyCheckProxyTests()
    {
        _fakeChangeLogDao = new FakeChangeLogDao();
        _fakeVersionDao = new FakeVersionDao();
        _fakeProductDao = new FakeProductDao();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
    }

    private ChangeLogLineReadonlyCheckProxy CreateProxy() =>
        new ChangeLogLineReadonlyCheckProxy(_fakeChangeLogDao, _fakeVersionDao, _memoryCache, _fakeProductDao);

    [Fact]
    public async Task AddLine_RelatedVersionAlreadyReleased_Conflict()
    {
        // arrange
        var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
        var version = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty, DateTime.Parse("2021-04-17"), TestAccount.UserId,
            DateTime.Parse("2021-04-17"), null);
        _fakeVersionDao.Versions.Add(version);

        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var line = new ChangeLogLine(lineId, versionId, TestAccount.Product.Id, ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));

        var proxy = CreateProxy();

        // act
        var result = await proxy.AddOrUpdateLineAsync(line);

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<VersionReleasedConflict>();
        _fakeChangeLogDao.ChangeLogs.Should().BeEmpty();
    }

    [Fact]
    public async Task AddLine_ChangeLogLineDeleted_Conflict()
    {
        // arrange
        var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
        var version = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty,
            DateTime.Parse("2021-04-17"), TestAccount.UserId, DateTime.Parse("2021-04-17"), null);
        _fakeVersionDao.Versions.Add(version);

        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var line = new ChangeLogLine(lineId, versionId, TestAccount.Product.Id, ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"), DateTime.Parse("2021-04-17"));

        var proxy = CreateProxy();

        // act
        var result = await proxy.AddOrUpdateLineAsync(line);

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ChangeLogLineDeletedConflict>();
        _fakeChangeLogDao.ChangeLogs.Should().BeEmpty();
    }

    [Fact]
    public async Task AddLine_RelatedVersionIsDeleted_Conflict()
    {
        // arrange
        var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
        var version = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty, null, TestAccount.UserId, DateTime.Parse("2021-04-17"),
            DateTime.Parse("2021-04-17"));
        _fakeVersionDao.Versions.Add(version);

        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var line = new ChangeLogLine(lineId, versionId, TestAccount.Product.Id, ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));

        var proxy = CreateProxy();

        // act
        var result = await proxy.AddOrUpdateLineAsync(line);

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<VersionDeletedConflict>();
        _fakeChangeLogDao.ChangeLogs.Should().BeEmpty();
    }

    [Fact]
    public async Task AddLine_RelatedProductIsFreezed_Conflict()
    {
        // arrange
        var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
        var version = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty,
            null, TestAccount.UserId, DateTime.Parse("2021-04-17"), null);
        _fakeVersionDao.Versions.Add(version);

        _fakeProductDao.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, Name.Parse("Test product"),
            TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
            DateTime.Parse("2021-05-13"),
            DateTime.Parse("2021-05-13")));

        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var line = new ChangeLogLine(lineId, versionId, TestAccount.Product.Id, ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));

        var proxy = CreateProxy();

        // act
        var result = await proxy.AddOrUpdateLineAsync(line);

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProductFreezedConflict>();
        _fakeChangeLogDao.ChangeLogs.Should().BeEmpty();
    }

    [Fact]
    public async Task AddPendingLine_RelatedProductIsFreezed_Conflict()
    {
        // arrange
        _fakeProductDao.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, Name.Parse("Test product"),
            TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
            DateTime.Parse("2021-05-13"),
            DateTime.Parse("2021-05-13")));

        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var line = new ChangeLogLine(lineId, null, TestAccount.Product.Id, ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));

        var proxy = CreateProxy();

        // act
        var result = await proxy.AddOrUpdateLineAsync(line);

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProductFreezedConflict>();
        _fakeChangeLogDao.ChangeLogs.Should().BeEmpty();
    }

    [Fact]
    public async Task AddLine_RelatedVersionIsNotReadOnly_SuccessfullyAdded()
    {
        // arrange
        var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
        var version = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty,
            null, TestAccount.UserId, DateTime.Parse("2021-04-17"), null);
        _fakeVersionDao.Versions.Add(version);

        _fakeProductDao.Products.Add(TestAccount.Product);

        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var line = new ChangeLogLine(lineId, versionId, TestAccount.Product.Id, ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));

        var proxy = CreateProxy();

        // act
        var result = await proxy.AddOrUpdateLineAsync(line);

        // assert
        result.IsSuccess.Should().BeTrue();
        _fakeChangeLogDao.ChangeLogs.Should().Contain(x => x.Id == lineId);
    }

    [Fact]
    public async Task AddLine_ChangeLogLineIsPending_SuccessfullyAdded()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var line = new ChangeLogLine(lineId, null, TestAccount.Product.Id, ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));

        _fakeProductDao.Products.Add(TestAccount.Product);

        var proxy = CreateProxy();

        // act
        var result = await proxy.AddOrUpdateLineAsync(line);

        // assert
        result.IsSuccess.Should().BeTrue();
        _fakeChangeLogDao.ChangeLogs.Should().Contain(x => x.Id == lineId);
    }

    [Fact]
    public async Task UpdateLine_ChangeLogLineIsPending_SuccessfullyAdded()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var line = new ChangeLogLine(lineId, null, TestAccount.Product.Id, ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));

        _fakeProductDao.Products.Add(TestAccount.Product);

        var proxy = CreateProxy();

        // act
        var result = await proxy.UpdateLineAsync(line);

        // assert
        result.IsSuccess.Should().BeTrue();
        _fakeChangeLogDao.ChangeLogs.Should().Contain(x => x.Id == lineId);
    }

    [Fact]
    public async Task DeleteLine_ChangeLogLineIsPending_SuccessfullyDeleted()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var line = new ChangeLogLine(lineId, null, TestAccount.Product.Id, ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));

        _fakeChangeLogDao.ChangeLogs.Add(line);
        _fakeProductDao.Products.Add(TestAccount.Product);

        var proxy = CreateProxy();

        // act
        var result = await proxy.DeleteLineAsync(line);

        // assert
        result.IsSuccess.Should().BeTrue();
        _fakeChangeLogDao.ChangeLogs.Should().HaveCount(0);
    }

    [Fact]
    public async Task DeleteLine_RelatedVersionAlreadyReleased_NotDeleted()
    {
        // arrange
        var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var line = new ChangeLogLine(lineId, versionId, TestAccount.Product.Id, ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));
        _fakeChangeLogDao.ChangeLogs.Add(line);

        var version = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty,
            DateTime.Parse("2021-05-15"), TestAccount.UserId, DateTime.Parse("2021-04-17"), null);
        _fakeVersionDao.Versions.Add(version);

        var proxy = CreateProxy();

        // act
        var result = await proxy.DeleteLineAsync(line);

        // assert
        result.IsSuccess.Should().BeFalse();
        _fakeChangeLogDao.ChangeLogs.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddOrUpdateLines_ChangeLogLinesArePending_SuccessfullyAdded()
    {
        // arrange
        var firstLineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var firstLine = new ChangeLogLine(firstLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));

        var secondLineId = Guid.Parse("4d755355-b527-4a4d-afbe-ab00a025609e");
        var secondLine = new ChangeLogLine(secondLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some other text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));

        _fakeProductDao.Products.Add(TestAccount.Product);

        var proxy = CreateProxy();

        // act
        var result = await proxy.AddOrUpdateLinesAsync(new List<ChangeLogLine>(2) {firstLine, secondLine});

        // assert
        result.IsSuccess.Should().BeTrue();
        _fakeChangeLogDao.ChangeLogs.Should().Contain(x => x.Id == firstLineId);
        _fakeChangeLogDao.ChangeLogs.Should().Contain(x => x.Id == secondLineId);
        _fakeChangeLogDao.ChangeLogs.Count.Should().Be(2);
    }

    [Fact]
    public async Task AddOrUpdateLines_RelatedVersionOfSecondLineIsReadOnly_Conflict()
    {
        // arrange
        var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
        var version = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty,
            DateTime.Parse("2021-04-17"), TestAccount.UserId, DateTime.Parse("2021-04-17"), null);
        _fakeVersionDao.Versions.Add(version);

        var firstLineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var firstLine = new ChangeLogLine(firstLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));

        var secondLineId = Guid.Parse("4d755355-b527-4a4d-afbe-ab00a025609e");
        var secondLine = new ChangeLogLine(secondLineId, versionId, TestAccount.Product.Id,
            ChangeLogText.Parse("some other text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));

        _fakeProductDao.Products.Add(TestAccount.Product);

        var proxy = CreateProxy();

        // act
        var result = await proxy.AddOrUpdateLinesAsync(new List<ChangeLogLine>(2) {firstLine, secondLine});

        // assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeOfType<VersionReleasedConflict>();
        _fakeChangeLogDao.ChangeLogs.Should().BeEmpty();
    }

    [Fact]
    public async Task MoveLine_RelatedVersionIsNotYetReleased_SuccessfullyMoved()
    {
        // arrange
        var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
        var version = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty,
            null, TestAccount.UserId, DateTime.Parse("2021-04-17"), null);
        _fakeVersionDao.Versions.Add(version);

        _fakeProductDao.Products.Add(TestAccount.Product);

        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var lineUnassigned = new ChangeLogLine(lineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));
        _fakeChangeLogDao.ChangeLogs.Add(lineUnassigned);

        var lineAssigned = lineUnassigned.AssignToVersion(versionId, 0);

        var proxy = CreateProxy();

        // act
        var result = await proxy.MoveLineAsync(lineAssigned);

        // assert
        result.IsSuccess.Should().BeTrue();
        _fakeChangeLogDao.ChangeLogs.Single().VersionId!.Value.Should().Be(versionId);
    }

    [Fact]
    public async Task MoveLines_RelatedVersionIsNotYetReleased_SuccessfullyMoved()
    {
        // arrange
        var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
        var version = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty,
            null, TestAccount.UserId, DateTime.Parse("2021-04-17"), null);
        _fakeVersionDao.Versions.Add(version);

        _fakeProductDao.Products.Add(TestAccount.Product);

        var firstLineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var firstLineUnassigned = new ChangeLogLine(firstLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));
        _fakeChangeLogDao.ChangeLogs.Add(firstLineUnassigned);

        var secondLineId = Guid.Parse("4d755355-b527-4a4d-afbe-ab00a025609e");
        var secondLineUnassigned = new ChangeLogLine(secondLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some other text"),
            1U, TestAccount.UserId, DateTime.Parse("2021-04-17"));
        _fakeChangeLogDao.ChangeLogs.Add(secondLineUnassigned);

        var firstLineAssigned = firstLineUnassigned.AssignToVersion(versionId, 0);
        var secondLineAssigned = secondLineUnassigned.AssignToVersion(versionId, 1);

        var proxy = CreateProxy();

        // act
        var result = await proxy.MoveLinesAsync(new List<ChangeLogLine>(2)
            {firstLineAssigned, secondLineAssigned});

        // assert
        result.IsSuccess.Should().BeTrue();
        _fakeChangeLogDao.ChangeLogs.All(x => x.VersionId == versionId).Should().BeTrue();
    }

    [Fact]
    public async Task MoveLines_RelatedVersionIsReleased_SuccessfullyMoved()
    {
        // arrange
        var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
        var version = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty,
            DateTime.Parse("2021-04-17"), TestAccount.UserId, DateTime.Parse("2021-04-17"), null);
        _fakeVersionDao.Versions.Add(version);

        var firstLineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var firstLineUnassigned = new ChangeLogLine(firstLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"),
            0U, TestAccount.UserId, DateTime.Parse("2021-04-17"));
        _fakeChangeLogDao.ChangeLogs.Add(firstLineUnassigned);

        var secondLineId = Guid.Parse("4d755355-b527-4a4d-afbe-ab00a025609e");
        var secondLineUnassigned = new ChangeLogLine(secondLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some other text"),
            1U, TestAccount.UserId, DateTime.Parse("2021-04-17"));
        _fakeChangeLogDao.ChangeLogs.Add(secondLineUnassigned);

        var firstLineAssigned = firstLineUnassigned.AssignToVersion(versionId, 0);
        var secondLineAssigned = secondLineUnassigned.AssignToVersion(versionId, 1);

        var proxy = CreateProxy();

        // act
        var result = await proxy.MoveLinesAsync(new List<ChangeLogLine>(2)
            {firstLineAssigned, secondLineAssigned});

        // assert
        result.IsSuccess.Should().BeFalse();
    }
}