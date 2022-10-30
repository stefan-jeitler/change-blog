using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Accounts.DeleteAccount;
using ChangeBlog.Domain;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Accounts.DeleteAccount;

public class DeleteAccountInteractorTests
{
    private readonly FakeAccountDao _accountDao;
    private readonly Mock<IDeleteAccountOutputPort> _outputPortMock;

    public DeleteAccountInteractorTests()
    {
        _outputPortMock = new Mock<IDeleteAccountOutputPort>(MockBehavior.Strict);
        _accountDao = new FakeAccountDao();
    }

    private DeleteAccountInteractor CreateInteractor() => new(_accountDao);

    [Fact]
    public async Task DeleteAccount_HappyPath_SuccessfullyDeleted()
    {
        // arrange
        _accountDao.Accounts.Add(TestAccount.Account);
        _outputPortMock.Setup(m => m.AccountDeleted(It.IsAny<Guid>()));
        var interactor = CreateInteractor();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.Id);

        // assert
        _outputPortMock.Verify(m => m.AccountDeleted(It.Is<Guid>(x => x == TestAccount.Id)), Times.Once);
    }

    [Fact]
    public async Task DeleteAccount_AccountAlreadyDeleted_AccountDeletedOutput()
    {
        // arrange
        _accountDao.Accounts.Add(new Account(TestAccount.Id,
            TestAccount.Name,
            null,
            TestAccount.CreationDate,
            TestAccount.CreatedByUser,
            DateTime.Parse("2022-10-31")));

        _outputPortMock.Setup(m => m.AccountDeleted(It.IsAny<Guid>()));
        var interactor = CreateInteractor();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.Id);

        // assert
        _outputPortMock.Verify(m => m.AccountDeleted(It.Is<Guid>(x => x == TestAccount.Id)), Times.Once);
    }
}