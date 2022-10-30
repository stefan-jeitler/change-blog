using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Accounts.UpdateAccount;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Accounts.UpdateAccount;

public class UpdateAccountInteractorTests
{
    private readonly FakeAccountDao _fakeAccountDao;
    private readonly Mock<IUpdateAccountOutputPort> _outputPortMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateAccountInteractorTests()
    {
        _fakeAccountDao = new FakeAccountDao();
        _outputPortMock = new Mock<IUpdateAccountOutputPort>(MockBehavior.Strict);
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    private UpdateAccountInteractor CreateInteractor() => new(_fakeAccountDao, _unitOfWorkMock.Object);

    [Fact]
    public async Task UpdateName_HappyPath_NameSuccessfullyUpdated()
    {
        // arrange
        _fakeAccountDao.Accounts.Add(TestAccount.Account);
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.Updated(It.IsAny<Guid>()));
        var requestModel = new UpdateAccountRequestModel(TestAccount.Id, "New Account Name");

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        _outputPortMock.Verify(m => m.Updated(It.Is<Guid>(x => x == TestAccount.Id)), Times.Once);
        _fakeAccountDao.Accounts.Should().HaveCount(1);
        _fakeAccountDao.Accounts.First().Name.Value.Should().Be("New Account Name");
    }

    [Fact]
    public async Task UpdateName_NameAlreadyTakenByAnotherAccount_NameAlreadyTakenOutput()
    {
        // arrange
        const string existingName = "Existing Name";
        _fakeAccountDao.Accounts.Add(TestAccount.Account);
        _fakeAccountDao.Accounts.Add(new Account(Guid.Parse("A26019E7-3BED-41D4-9F32-F779D6484D60"),
            Name.Parse(existingName),
            null,
            TestAccount.Account.CreatedAt,
            TestAccount.CreatedByUser,
            null));

        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.NewNameAlreadyTaken(It.IsAny<string>()));
        var requestModel = new UpdateAccountRequestModel(TestAccount.Id, existingName);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock
            .Verify(m => m.NewNameAlreadyTaken(It.Is<string>(x => x == existingName)), Times.Once);
    }

    [Fact]
    public async Task UpdateName_BadName_InvalidNameOutput()
    {
        // arrange
        const string invalidName = "a";
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.InvalidName(It.IsAny<string>()));
        var requestModel = new UpdateAccountRequestModel(TestAccount.Id, invalidName);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock
            .Verify(m => m.InvalidName(It.Is<string>(x => x == invalidName)), Times.Once);
    }
}