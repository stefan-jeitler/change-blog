using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Accounts.CreateAccount;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Accounts.CreateAccount;

public class CreateAccountInteractorTests
{
    private readonly FakeAccountDao _accountDao;
    private readonly Mock<ICreateAccountOutputPort> _outputPortMock;
    private readonly FakeRolesDao _rolesDao;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly FakeUserDao _userDao;

    public CreateAccountInteractorTests()
    {
        _accountDao = new FakeAccountDao();
        _rolesDao = new FakeRolesDao();
        _userDao = new FakeUserDao();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _outputPortMock = new Mock<ICreateAccountOutputPort>(MockBehavior.Strict);
    }

    private CreateAccountInteractor CreateInteractor() =>
        new(_accountDao, _userDao, _rolesDao, _unitOfWorkMock.Object);

    [Fact]
    public async Task CreateAccount_HappyPath_Successfully()
    {
        // arrange
        _rolesDao.Roles.AddRange(TestRoles.Roles);
        _userDao.Users.Add(TestAccount.User);
        var requestModel = new CreateAccountRequestModel("TestAccount", TestAccount.UserId);
        _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
        var sut = CreateInteractor();

        // act
        await sut.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        var createdAccount = _accountDao.Accounts.Single();
        _outputPortMock
            .Verify(m => m.Created(It.Is<Guid>(g => g == createdAccount.Id)), Times.Once);
    }

    [Fact]
    public async Task CreateAccount_BadName_InvalidNameOutput()
    {
        // arrange
        var requestModel = new CreateAccountRequestModel(" ", TestAccount.UserId);
        _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
        _outputPortMock.Setup(m => m.InvalidName(It.IsAny<string>()));
        var sut = CreateInteractor();

        // act
        await sut.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Never);
        _outputPortMock.Verify(m => m.InvalidName(It.Is<string>(n => n == " ")), Times.Once);
    }

    [Fact]
    public async Task CreateAccount_AccountWithSameNameExists_AccountAlreadyExistsOutput()
    {
        // arrange
        _rolesDao.Roles.AddRange(TestRoles.Roles);
        _userDao.Users.Add(TestAccount.User);
        _accountDao.Accounts.Add(TestAccount.Account);
        var requestModel = new CreateAccountRequestModel(TestAccount.Name, TestAccount.UserId);
        _outputPortMock.Setup(m => m.AccountAlreadyExists(It.IsAny<Guid>()));
        var sut = CreateInteractor();

        // act
        await sut.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock
            .Verify(m => m.AccountAlreadyExists(It.Is<Guid>(g => g == TestAccount.Id)), Times.Once);
    }

    [Fact]
    public async Task CreateAccount_MaxCountAlreadyReached_TooManyAccountsCreatedOutput()
    {
        // arrange
        _rolesDao.Roles.AddRange(TestRoles.Roles);
        _userDao.Users.Add(TestAccount.User);

        var accounts = Enumerable.Range(0, 5).Select(x =>
            new Account(Guid.NewGuid(), Name.Parse($"{x:0000}"), null, DateTime.UtcNow, TestAccount.UserId,
                null));

        _accountDao.Accounts.AddRange(accounts);
        var requestModel = new CreateAccountRequestModel(TestAccount.Name, TestAccount.UserId);
        _outputPortMock.Setup(m => m.TooManyAccountsCreated(It.IsAny<ushort>()));
        var sut = CreateInteractor();

        // act
        await sut.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock
            .Verify(m => m.TooManyAccountsCreated(It.Is<ushort>(l => l == 5)), Times.Once);
    }
}