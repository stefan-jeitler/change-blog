using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion;
using ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.Models;
using ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.OutputPorts;
using ChangeBlog.Domain;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.AddVersion;

public class AddVersionInteractorTests
{
    private readonly FakeChangeLogDao _fakeChangeLogDao;
    private readonly FakeProductDao _fakeProductDao;
    private readonly FakeVersionDao _fakeVersionDao;
    private readonly Mock<IAddVersionOutputPort> _outputPortMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public AddVersionInteractorTests()
    {
        _fakeProductDao = new FakeProductDao();
        _fakeVersionDao = new FakeVersionDao();
        _fakeChangeLogDao = new FakeChangeLogDao();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _outputPortMock = new Mock<IAddVersionOutputPort>(MockBehavior.Strict);
    }

    private AddOrUpdateVersionInteractor CreateInteractor()
    {
        return new(_fakeProductDao, _fakeVersionDao,
            _unitOfWorkMock.Object, _fakeChangeLogDao, _fakeChangeLogDao);
    }

    [Fact]
    public async Task AddVersion_ValidVersion_Successful()
    {
        // arrange
        var changeLogLines = new List<ChangeLogLineRequestModel>
        {
            new("Proxy bug resolved", new List<string> { "ProxyStrikesBack" }, new List<string> { "#123" }),
            new("New feature added", new List<string> { "Feature" }, new List<string>()),
            new("Allow https only", new List<string> { "Security" }, new List<string>())
        };
        var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id,
            "1.23", "", changeLogLines);

        _fakeProductDao.Products.Add(TestAccount.Product);

        _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
        var version = _fakeVersionDao.Versions.Single(x => x.ProductId == TestAccount.Product.Id);
        version.Value.Should().Be(ClVersionValue.Parse("1.23"));
        version.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task AddVersion_NotExistingProduct_ProductDoesNotExistOutput()
    {
        // arrange
        var changeLogLines = new List<ChangeLogLineRequestModel>
        {
            new("Proxy bug resolved", new List<string> { "ProxyStrikesBack" }, new List<string> { "#123" }),
            new("New feature added", new List<string> { "Feature" }, new List<string>()),
            new("Allow https only", new List<string> { "Security" }, new List<string>())
        };
        var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id,
            "1.23", OptionalName.Empty, changeLogLines);


        _outputPortMock.Setup(m => m.ProductDoesNotExist(It.IsAny<Guid>()));
        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        _outputPortMock.Verify(m => m.ProductDoesNotExist(It.Is<Guid>(x => x == TestAccount.Product.Id)),
            Times.Once);
    }

    [Fact]
    public async Task AddVersion_ProductIsClosed_ProductClosedOutput()
    {
        // arrange
        var changeLogLines = new List<ChangeLogLineRequestModel>
        {
            new("Proxy bug resolved", new List<string> { "ProxyStrikesBack" }, new List<string> { "#123" })
        };
        var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id,
            "1.23", OptionalName.Empty, changeLogLines);

        _fakeProductDao.Products.Add(new Product(TestAccount.Product.Id,
            TestAccount.Id,
            Name.Parse("Test product"),
            TestAccount.CustomVersioningScheme,
            TestAccount.Product.LanguageCode,
            TestAccount.UserId,
            DateTime.Parse("2021-05-13"),
            DateTime.Parse("2021-05-13")));

        _outputPortMock.Setup(m => m.RelatedProductClosed(It.IsAny<Guid>()));
        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        _outputPortMock.Verify(m => m.RelatedProductClosed(It.Is<Guid>(x => x == TestAccount.Product.Id)),
            Times.Once);
    }

    [Fact]
    public async Task AddVersion_VersionExists_VersionAlreadyExistsOutput()
    {
        // arrange
        var changeLogLines = new List<ChangeLogLineRequestModel>
        {
            new("Proxy bug resolved", new List<string> { "ProxyStrikesBack" }, new List<string> { "#123" })
        };
        var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id,
            "1.23", OptionalName.Empty, changeLogLines);

        _fakeProductDao.Products.Add(TestAccount.Product);
        var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"),
            OptionalName.Empty, TestAccount.UserId);
        _fakeVersionDao.Versions.Add(clVersion);

        _outputPortMock.Setup(m => m.VersionAlreadyExists(It.IsAny<Guid>()));
        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        _outputPortMock.Verify(m => m.VersionAlreadyExists(It.Is<Guid>(x => x == clVersion.Id)), Times.Once);
    }

    [Fact]
    public async Task AddVersion_InvalidVersionFormat_InvalidVersionFormatOutput()
    {
        // arrange
        var changeLogLines = new List<ChangeLogLineRequestModel>
        {
            new("Proxy bug resolved", new List<string> { "ProxyStrikesBack" }, new List<string> { "#123" }),
            new("New feature added", new List<string> { "Feature" }, new List<string>()),
            new("Allow https only", new List<string> { "Security" }, new List<string>())
        };
        var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id,
            "1. .2", OptionalName.Empty, changeLogLines);

        _fakeProductDao.Products.Add(TestAccount.Product);
        _outputPortMock.Setup(m => m.InvalidVersionFormat(It.IsAny<string>()));
        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        _outputPortMock.Verify(m =>
            m.InvalidVersionFormat(It.Is<string>(x => x == "1. .2")), Times.Once);
    }

    [Fact]
    public async Task AddVersion_VersionDoesNotMachScheme_InvalidVersionFormatOutput()
    {
        // arrange
        var changeLogLines = new List<ChangeLogLineRequestModel>
        {
            new("Proxy bug resolved", new List<string> { "ProxyStrikesBack" }, new List<string> { "#123" }),
            new("New feature added", new List<string> { "Feature" }, new List<string>()),
            new("Allow https only", new List<string> { "Security" }, new List<string>())
        };
        var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id,
            "*.23", OptionalName.Empty, changeLogLines);

        _fakeProductDao.Products.Add(TestAccount.Product);
        _outputPortMock.Setup(m => m.VersionDoesNotMatchScheme(It.IsAny<string>(), It.IsAny<string>()));
        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        _outputPortMock.Verify(m =>
            m.VersionDoesNotMatchScheme(It.Is<string>(x => x == "*.23"), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task AddVersion_TooManyChangeLogLines_TooManyLinesOutput()
    {
        // arrange
        var changeLogLines = Enumerable.Range(0, 101)
            .Select(x =>
                new ChangeLogLineRequestModel($"{x:D5}", new List<string> { "Security" }, new List<string>()))
            .ToList();

        var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id,
            "1.23", OptionalName.Empty, changeLogLines);

        _fakeProductDao.Products.Add(TestAccount.Product);
        _outputPortMock.Setup(m => m.TooManyLines(It.IsAny<int>()));
        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        _outputPortMock.Verify(m =>
            m.TooManyLines(It.Is<int>(x => x == ChangeLogs.MaxLines)), Times.Once);
    }

    [Fact]
    public async Task CreateVersion_ReleaseImmediately_ReleaseDateIsNotNull()
    {
        // arrange
        var changeLogLines = new List<ChangeLogLineRequestModel>
        {
            new("Proxy bug resolved", new List<string> { "ProxyStrikesBack" }, new List<string> { "#123" }),
            new("New feature added", new List<string> { "Feature" }, new List<string>()),
            new("Allow https only", new List<string> { "Security" }, new List<string>())
        };
        var versionRequestModel =
            new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.23", OptionalName.Empty,
                changeLogLines, true);

        _fakeProductDao.Products.Add(TestAccount.Product);
        _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        _fakeVersionDao.Versions.Single().ReleasedAt.HasValue.Should().BeTrue();
    }

    [Fact]
    public async Task AddVersion_LinesWithSameText_SameTextsAreNotAllowed()
    {
        // arrange
        var changeLogLines = new List<ChangeLogLineRequestModel>
        {
            new("Proxy bug resolved", new List<string> { "ProxyStrikesBack" }, new List<string> { "#123" }),
            new("New feature added", new List<string> { "Feature" }, new List<string>()),
            new("Allow https only", new List<string> { "Security" }, new List<string>()),
            new("Allow https only", new List<string> { "Security" }, new List<string> { "#1234" })
        };
        var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id,
            "1.23", OptionalName.Empty, changeLogLines);

        _fakeProductDao.Products.Add(TestAccount.Product);
        _outputPortMock.Setup(m => m.LinesWithSameTextsAreNotAllowed(It.IsAny<List<string>>()));
        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        _outputPortMock.Verify(m =>
            m.LinesWithSameTextsAreNotAllowed(It.Is<List<string>>(x =>
                x.Count == 1 && x.Contains("allow https only"))));
    }

    [Fact]
    public async Task AddVersion_DoNotReleaseVersionYet_ReleaseDateIsNull()
    {
        // arrange
        var changeLogLines = new List<ChangeLogLineRequestModel>
        {
            new("Proxy bug resolved", new List<string> { "ProxyStrikesBack" }, new List<string> { "#123" }),
            new("New feature added", new List<string> { "Feature" }, new List<string>())
        };
        var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id,
            "1.23", OptionalName.Empty, changeLogLines);

        _fakeProductDao.Products.Add(TestAccount.Product);
        _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        _fakeVersionDao.Versions.Single().ReleasedAt.HasValue.Should().BeFalse();
    }

    [Fact]
    public async Task AddVersion_ValidVersion_PositionsProperlySet()
    {
        // arrange
        var changeLogLines = Enumerable.Range(0, 50)
            .Select(x =>
                new ChangeLogLineRequestModel($"{x:D5}", new List<string> { "Security" }, new List<string>()))
            .ToList();

        var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id,
            "1.23", OptionalName.Empty, changeLogLines);

        _fakeProductDao.Products.Add(TestAccount.Product);
        _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        foreach (var (lineRequestModel, i) in changeLogLines.Select((x, i) => (x, i)))
            _fakeChangeLogDao.ChangeLogs[i].Position.Should().Be(uint.Parse(lineRequestModel.Text));
    }

    [Fact]
    public async Task AddVersion_ValidVersion_TransactionStartedAndCommitted()
    {
        // arrange
        var changeLogLines = new List<ChangeLogLineRequestModel>
        {
            new("Proxy bug resolved", new List<string> { "ProxyStrikesBack" }, new List<string> { "#123" }),
            new("New feature added", new List<string> { "Feature" }, new List<string>()),
            new("Allow https only", new List<string> { "Security" }, new List<string>())
        };
        var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id,
            "1.23", OptionalName.Empty, changeLogLines);

        _fakeProductDao.Products.Add(TestAccount.Product);
        _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
    }

    [Fact]
    public async Task AddVersion_ConflictWhileSaving_ConflictOutput()
    {
        // arrange
        var changeLogLines = new List<ChangeLogLineRequestModel>
        {
            new("Proxy bug resolved", new List<string> { "ProxyStrikesBack" }, new List<string> { "#123" }),
            new("New feature added", new List<string> { "Feature" }, new List<string>()),
            new("Allow https only", new List<string> { "Security" }, new List<string>())
        };
        var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id,
            "1.23", OptionalName.Empty, changeLogLines);

        _fakeChangeLogDao.Conflict = new ConflictStub();
        _fakeProductDao.Products.Add(TestAccount.Product);
        _outputPortMock.Setup(m => m.InsertConflict(It.IsAny<Conflict>()));
        _unitOfWorkMock.Setup(m => m.Start());

        var addVersionInteractor = CreateInteractor();

        // act
        await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

        // assert
        _outputPortMock.Verify(m => m.InsertConflict(It.IsAny<Conflict>()), Times.Once);
    }
}