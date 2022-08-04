using System.Threading.Tasks;
using ChangeBlog.Application.Extensions;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Accounts.GetAccounts;
using ChangeBlog.Domain;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Queries.GetAccounts;

public class GetAccountsInteractorTests
{
    private readonly FakeAccountDao _fakeAccountDao;
    private readonly FakeUserDao _fakeUserDao;
    private readonly FakeVersioningSchemeDao _fakeVersioningSchemeDao;

    public GetAccountsInteractorTests()
    {
        _fakeAccountDao = new FakeAccountDao();
        _fakeUserDao = new FakeUserDao();
        _fakeVersioningSchemeDao = new FakeVersioningSchemeDao();
    }

    private GetAccountsInteractor CreateInteractor()
    {
        return new GetAccountsInteractor(_fakeAccountDao, _fakeUserDao, _fakeVersioningSchemeDao);
    }

    [Fact]
    public async Task GetAccounts_HappyPath_Successful()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeAccountDao.Accounts.Add(TestAccount.Account);
        _fakeVersioningSchemeDao.VersioningSchemes.Add(TestAccount.CustomVersioningScheme);
        _fakeVersioningSchemeDao.VersioningSchemes.Add(TestAccount.DefaultScheme);
        var interactor = CreateInteractor();

        // act 
        var accounts = await interactor.ExecuteAsync(TestAccount.UserId);

        // assert
        accounts.Should().HaveCount(1);
        accounts.Should().Contain(x => x.Id == TestAccount.Id);
    }

    [Fact]
    public async Task GetAccounts_CreatedAt_ConvertedToLocalTime()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeAccountDao.Accounts.Add(TestAccount.Account);
        _fakeVersioningSchemeDao.VersioningSchemes.Add(TestAccount.CustomVersioningScheme);
        _fakeVersioningSchemeDao.VersioningSchemes.Add(TestAccount.DefaultScheme);
        var interactor = CreateInteractor();
        var createdAtLocal = TestAccount.CreationDate.ToLocal(TestAccount.User.TimeZone);

        // act 
        var accounts = await interactor.ExecuteAsync(TestAccount.UserId);

        // assert
        accounts.Should().HaveCount(1);
        accounts.Should().Contain(x => x.CreatedAt == createdAtLocal);
    }

    [Fact]
    public async Task GetAccount_HappyPath_Successful()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeAccountDao.Accounts.Add(TestAccount.Account);
        _fakeVersioningSchemeDao.VersioningSchemes.Add(TestAccount.CustomVersioningScheme);
        var interactor = CreateInteractor();

        // act 
        var account = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Id);

        // assert
        account.Id.Should().Be(TestAccount.Id);
    }

    [Fact]
    public async Task GetAccount_AccountWithoutDefaultScheme_UsesGlobalDefaultScheme()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeAccountDao.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
            null));
        _fakeVersioningSchemeDao.VersioningSchemes.Add(TestAccount.DefaultScheme);
        var interactor = CreateInteractor();

        // act 
        var account = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Id);

        // assert
        account.DefaultVersioningSchemeId.Should().Be(TestAccount.DefaultScheme.Id);
    }
}