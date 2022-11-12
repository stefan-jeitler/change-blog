using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Extensions;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Products.GetProducts;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Products.GetProducts;

public class GetProductsInteractorTests
{
    private readonly FakeAccountDao _fakeAccountDao;
    private readonly FakeProductDao _fakeProductDao;
    private readonly FakeUserDao _fakeUserDao;

    public GetProductsInteractorTests()
    {
        _fakeProductDao = new FakeProductDao();
        _fakeUserDao = new FakeUserDao();
        _fakeAccountDao = new FakeAccountDao();
        _fakeAccountDao.Accounts.Add(TestAccount.Account);
    }

    private GetProductsInteractor CreateInteractor() => new(_fakeProductDao, _fakeUserDao, _fakeAccountDao);

    [Fact]
    public async Task GetAccountProducts_HappyPath_Successful()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeProductDao.Products.Add(TestAccount.Product);
        var interactor = CreateInteractor();
        var requestModel = new AccountProductQueryRequestModel(TestAccount.UserId, TestAccount.Id, null, null, 1, true);

        // act
        var products = await interactor.ExecuteAsync(requestModel);

        // assert
        products.Should().HaveCount(1);
        products.Should().ContainSingle(x => x.Id == TestAccount.Product.Id);
    }

    [Fact]
    public async Task GetAccountProducts_NotExistingAccountId_EmptyResult()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeProductDao.Products.Add(TestAccount.Product);
        var interactor = CreateInteractor();
        var notExistingAccountId = Guid.Parse("3639c610-bd58-4924-a5fa-ec19b3a324b0");
        var requestModel =
            new AccountProductQueryRequestModel(TestAccount.UserId, notExistingAccountId, null, null, 1, true);

        // act
        var products = await interactor.ExecuteAsync(requestModel);

        // assert
        products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProduct_HappyPath_Successful()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeProductDao.Products.Add(TestAccount.Product);
        var interactor = CreateInteractor();

        // act
        var product = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Product.Id);

        // assert
        product.HasValue.Should().BeTrue();
    }

    [Fact]
    public async Task GetProduct_CreateAt_ProperlyConvertedToUserTimeZone()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeProductDao.Products.Add(TestAccount.Product);
        var interactor = CreateInteractor();
        var createdAtLocal = TestAccount.Product.CreatedAt.ToLocal(TestAccount.User.TimeZone);

        // act
        var product = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Product.Id);

        // assert
        product.GetValueOrThrow().CreatedAt.Should().Be(createdAtLocal);
    }


    [Fact]
    public async Task GetUserProducts_HappyPath_Successful()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeProductDao.Products.Add(TestAccount.Product);
        var interactor = CreateInteractor();
        var requestModel = new UserProductQueryRequestModel(TestAccount.UserId, null, null, 1, true);

        // act
        var products = await interactor.ExecuteAsync(requestModel);

        // assert
        products.Should().HaveCount(1);
        products.Should().ContainSingle(x => x.Id == TestAccount.Product.Id);
        products.Should().ContainSingle(x => x.AccountId == TestAccount.Id);
        products.Should().ContainSingle(x => x.VersioningSchemeId == TestAccount.Product.VersioningScheme.Id);
        products.Should().ContainSingle(x => x.Name == TestAccount.Product.Name.Value);
        products.Should().ContainSingle(x => x.FreezedAt == TestAccount.Product.FreezedAt);
        products.Should().ContainSingle(x => x.CreatedByUser == TestAccount.User.Email.Value);
    }

    [Fact]
    public async Task GetUserProducts_NotProductsExist_EmptyResult()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        var interactor = CreateInteractor();
        var requestModel = new UserProductQueryRequestModel(TestAccount.User.Id, null, null, 1, true);

        // act
        var products = await interactor.ExecuteAsync(requestModel);

        // assert
        products.Should().BeEmpty();
    }
}