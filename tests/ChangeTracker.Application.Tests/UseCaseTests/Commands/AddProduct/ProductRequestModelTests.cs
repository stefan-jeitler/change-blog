using System;
using ChangeTracker.Application.UseCases.Commands.AddProduct;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AddProduct
{
    public class ProductRequestModelTests
    {
        private Guid _testAccountId;
        private string _testName;
        private Guid? _testSchemeId;
        private Guid _testUserId;

        public ProductRequestModelTests()
        {
            _testAccountId = Guid.Parse("c1b588ee-069d-453e-8f74-fc43b7ae0649");
            _testName = "Product X";
            _testSchemeId = Guid.Parse("76a96500-6446-42b3-bb3d-5e318b338b0d");
            _testUserId = Guid.Parse("a1b89f2d-d13f-4572-8522-8a92fb4fdb6a");
        }

        private ProductRequestModel CreateRequestModel() => new(_testAccountId, _testName, _testSchemeId, _testUserId);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.Name.Should().Be(_testName);
            requestModel.AccountId.Should().Be(_testAccountId);
            requestModel.VersioningSchemeId.HasValue.Should().BeTrue();
            requestModel.VersioningSchemeId.Should().HaveValue();
            requestModel.VersioningSchemeId!.Value.Should().Be(_testSchemeId!.Value);
        }

        [Fact]
        public void Create_WithEmptyAccountId_ArgumentException()
        {
            _testAccountId = Guid.Empty;

            Func<ProductRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullName_ArgumentNullException()
        {
            _testName = null;

            Func<ProductRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithEmptySchemeId_ArgumentNullException()
        {
            _testSchemeId = Guid.Empty;

            Func<ProductRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyUserId_ArgumentNullException()
        {
            _testUserId = Guid.Empty;

            Func<ProductRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}