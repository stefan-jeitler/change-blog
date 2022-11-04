using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Products.FreezeProduct;
using ChangeBlog.Domain;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Products.FreezeProduct;

public class FreezeProductInteractorTests
{
    private readonly FakeProductDao _fakeProductDao;
    private readonly Mock<IFreezeProductOutputPort> _outputPortMock;

    public FreezeProductInteractorTests()
    {
        _fakeProductDao = new FakeProductDao();
        _outputPortMock = new Mock<IFreezeProductOutputPort>(MockBehavior.Strict);
    }

    private FreezeProductInteractor CreateInteractor() => new(_fakeProductDao);

    [Fact]
    public async Task FreezeProduct_ProductDoesNotExist_ProductDoesNotExistOutput()
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
    public async Task FreezeProduct_ProductAlreadyFreezed_ProductAlreadyFreezedOutput()
    {
        // arrange
        var product = new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
            TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
            TestAccount.Product.CreatedAt,
            DateTime.Parse("2021-05-13"));
        _fakeProductDao.Products.Add(product);
        _outputPortMock.Setup(m => m.ProductAlreadyFreezed(It.IsAny<Guid>()));
        var interactor = CreateInteractor();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, product.Id);

        // assert
        _outputPortMock.Verify(m => m.ProductAlreadyFreezed(It.Is<Guid>(x => x == product.Id)), Times.Once);
    }

    [Fact]
    public async Task FreezeProduct_HappyPath_ProductFreezedOutput()
    {
        // arrange
        var product = new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
            TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
            TestAccount.Product.CreatedAt, null);
        _fakeProductDao.Products.Add(product);

        _outputPortMock.Setup(m => m.ProductFreezed(It.IsAny<Guid>()));
        var interactor = CreateInteractor();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, product.Id);

        // assert
        _outputPortMock.Verify(m => m.ProductFreezed(It.IsAny<Guid>()), Times.Once);
    }
}