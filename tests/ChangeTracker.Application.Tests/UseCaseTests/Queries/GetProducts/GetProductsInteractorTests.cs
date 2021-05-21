using System;
using System.Threading.Tasks;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Queries.GetProducts;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Queries.GetProducts
{
    public class GetProductsInteractorTests
    {
        private readonly ProductDaoStub _productDaoStub;
        private readonly UserDaoStub _userDaoStub;
        private readonly AccountDaoStub _accountDaoStub;

        public GetProductsInteractorTests()
        {
            _productDaoStub = new ProductDaoStub();
            _userDaoStub = new UserDaoStub();
            _accountDaoStub = new AccountDaoStub();
            _accountDaoStub.Accounts.Add(TestAccount.Account);
        }

        private GetProductsInteractor CreateInteractor()
        {
            return new(_productDaoStub, _userDaoStub, _accountDaoStub);
        }

        [Fact]
        public async Task GetAccountProducts_HappyPath_Successful()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);
            _productDaoStub.Products.Add(TestAccount.Product);
            var interactor = CreateInteractor();
            var requestModel = new AccountProductQueryRequestModel(TestAccount.UserId, TestAccount.Id, null, 1, true);

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
            _userDaoStub.Users.Add(TestAccount.User);
            _productDaoStub.Products.Add(TestAccount.Product);
            var interactor = CreateInteractor();
            var notExistingAccountId = Guid.Parse("3639c610-bd58-4924-a5fa-ec19b3a324b0");
            var requestModel = new AccountProductQueryRequestModel(TestAccount.UserId, notExistingAccountId, null, 1, true);

            // act
            var products = await interactor.ExecuteAsync(requestModel);

            // assert
            products.Should().BeEmpty();
        }

        [Fact]
        public async Task GetProduct_HappyPath_Successful()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);
            _productDaoStub.Products.Add(TestAccount.Product);
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
            _userDaoStub.Users.Add(TestAccount.User);
            _productDaoStub.Products.Add(TestAccount.Product);
            var interactor = CreateInteractor();
            var createdAtLocal = TestAccount.Product.CreatedAt.ToLocal(TestAccount.User.TimeZone);

            // act
            var product = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Product.Id);

            // assert
            product.Value.CreatedAt.Should().Be(createdAtLocal);
        }
        
        
        [Fact]
        public async Task GetUserProducts_HappyPath_Successful()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);
            _productDaoStub.Products.Add(TestAccount.Product);
            var interactor = CreateInteractor();
            var requestModel = new UserProductQueryRequestModel(TestAccount.UserId, null, 1, true);

            // act
            var products = await interactor.ExecuteAsync(requestModel);

            // assert
            products.Should().HaveCount(1);
            products.Should().ContainSingle(x => x.Id == TestAccount.Product.Id);
            products.Should().ContainSingle(x => x.AccountId == TestAccount.Id);
            products.Should().ContainSingle(x => x.VersioningSchemeId == TestAccount.Product.VersioningScheme.Id);
            products.Should().ContainSingle(x => x.Name == TestAccount.Product.Name.Value);
            products.Should().ContainSingle(x => x.ClosedAt == TestAccount.Product.ClosedAt);
            products.Should().ContainSingle(x => x.CreatedByUser == TestAccount.User.Email.Value);
        }
        
        [Fact]
        public async Task GetUserProducts_NotProductsExist_EmptyResult()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);
            var interactor = CreateInteractor();
            var requestModel = new UserProductQueryRequestModel(TestAccount.User.Id, null, 1, true);

            // act
            var products = await interactor.ExecuteAsync(requestModel);

            // assert
            products.Should().BeEmpty();
        }

    }
}