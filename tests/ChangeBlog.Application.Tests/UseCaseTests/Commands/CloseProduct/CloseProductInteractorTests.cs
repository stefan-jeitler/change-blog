using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Commands.CloseProduct;
using ChangeBlog.Domain;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.CloseProduct;

public class CloseProductInteractorTests
{
    private readonly FakeProductDao _fakeProductDao;
    private readonly Mock<ICloseProductOutputPort> _outputPortMock;

    public CloseProductInteractorTests()
    {
        _fakeProductDao = new FakeProductDao();
        _outputPortMock = new Mock<ICloseProductOutputPort>(MockBehavior.Strict);
    }

    private CloseProductInteractor CreateInteractor()
    {
        return new(_fakeProductDao);
    }

    [Fact]
    public async Task CloseProduct_ProductDoesNotExist_ProductDoesNotExistOutput()
    {
        // arrange
        var notExistingProductId = Guid.Parse("658ab2ec-ac88-47aa-af14-2093a0d07f4f");
        _outputPortMock.Setup(m => m.ProductDoesNotExist(It.IsAny<Guid>()));
        var interactor = CreateInteractor();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, notExistingProductId);

        // assert
        _outputPortMock.Verify(m => m.ProductDoesNotExist(It.Is<Guid>(x => x == notExistingProductId)), Times.Once);
    }

    [Fact]
    public async Task CloseProduct_ProductAlreadyClosed_ProductAlreadyClosedOutput()
    {
        // arrange
        var product = new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
            TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
            TestAccount.Product.CreatedAt,
            DateTime.Parse("2021-05-13"));
        _fakeProductDao.Products.Add(product);
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
            TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
            TestAccount.Product.CreatedAt, null);
        _fakeProductDao.Products.Add(product);

        _outputPortMock.Setup(m => m.ProductClosed(It.IsAny<Guid>()));
        var interactor = CreateInteractor();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, product.Id);

        // assert
        _outputPortMock.Verify(m => m.ProductClosed(It.IsAny<Guid>()), Times.Once);
    }
}