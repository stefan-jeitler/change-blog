using System;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Queries.GetProducts;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Queries.GetProducts
{
    public class ProductsQueryRequestModelTests
    {
        private readonly Guid? _testLastProductId;
        private Guid _testAccountId;
        private ushort _testCount;

        private Guid _testUserId;


        public ProductsQueryRequestModelTests()
        {
            _testUserId = TestAccount.UserId;
            _testAccountId = TestAccount.Id;
            _testLastProductId = Guid.Parse("33f9a7a4-5a0b-4ac8-b074-d97e13e8596c");
            _testCount = 100;
        }

        private AccountProductQueryRequestModel CreateRequestModel() =>
            new(_testUserId, _testAccountId, _testLastProductId, _testCount, true);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.UserId.Should().Be(_testUserId);
            requestModel.AccountId.Should().Be(_testAccountId);
            requestModel.LastProductId.Should().Be(_testLastProductId);
            requestModel.Limit.Should().Be(_testCount);
            requestModel.IncludeClosedProducts.Should().Be(true);
        }

        [Fact]
        public void Create_WithEmptyUserId_ArgumentException()
        {
            // arrange
            _testUserId = Guid.Empty;

            // act
            Func<AccountProductQueryRequestModel> act = CreateRequestModel;

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyAccountId_ArgumentException()
        {
            // arrange
            _testAccountId = Guid.Empty;

            // act
            Func<AccountProductQueryRequestModel> act = CreateRequestModel;

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithTooHighCountValue_ValueCapped()
        {
            // arrange
            _testCount = 500;

            // act
            var requestModel = CreateRequestModel();

            // assert
            requestModel.Limit.Should().Be(AccountProductQueryRequestModel.MaxLimit);
        }
    }
}
