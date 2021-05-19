﻿using System;
using System.Threading.Tasks;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.CloseProduct;
using ChangeTracker.Domain;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.CloseProject
{
    public class CloseProductInteractorTests
    {
        private readonly Mock<ICloseProductOutputPort> _outputPortMock;
        private readonly ProductDaoStub _productDaoStub;

        public CloseProductInteractorTests()
        {
            _productDaoStub = new ProductDaoStub();
            _outputPortMock = new Mock<ICloseProductOutputPort>(MockBehavior.Strict);
        }

        private CloseProductInteractor CreateInteractor()
        {
            return new(_productDaoStub);
        }

        [Fact]
        public async Task CloseProduct_ProductDoesNotExist_ProductDoesNotExistOutput()
        {
            // arrange
            var notExistingProductId = Guid.Parse("658ab2ec-ac88-47aa-af14-2093a0d07f4f");
            _outputPortMock.Setup(m => m.ProductDoesNotExist());
            var interactor = CreateInteractor();

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, notExistingProductId);

            // assert
            _outputPortMock.Verify(m => m.ProductDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task CloseProduct_ProductAlreadyClosed_ProductAlreadyClosedOutput()
        {
            // arrange
            var product = new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, TestAccount.Product.CreatedAt,
                DateTime.Parse("2021-05-13"));
            _productDaoStub.Products.Add(product);
            _outputPortMock.Setup(m => m.ProductAlreadyClosed(It.IsAny<Guid>()));
            var interactor = CreateInteractor();

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, product.Id);

            // assert
            _outputPortMock.Verify(m => m.ProductAlreadyClosed(It.Is<Guid>(x => x == product.Id)), Times.Once);
        }

        [Fact]
        public async Task CloseProduct_HappyPath_ProductClosedOutput()
        {
            // arrange
            var product = new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, TestAccount.Product.CreatedAt, null);
            _productDaoStub.Products.Add(product);

            _outputPortMock.Setup(m => m.ProductClosed(It.IsAny<Guid>()));
            var interactor = CreateInteractor();

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, product.Id);

            // assert
            _outputPortMock.Verify(m => m.ProductClosed(It.IsAny<Guid>()), Times.Once);
        }
    }
}