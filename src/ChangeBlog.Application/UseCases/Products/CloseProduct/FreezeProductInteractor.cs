using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Products;

namespace ChangeBlog.Application.UseCases.Products.CloseProduct;

public class FreezeProductInteractor : IFreezeProduct
{
    private readonly IProductDao _productDao;

    public FreezeProductInteractor(IProductDao productDao)
    {
        _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
    }

    public async Task ExecuteAsync(IFreezeProductOutputPort output, Guid productId)
    {
        var product = await _productDao.FindProductAsync(productId);
        if (product.HasNoValue)
        {
            output.ProductDoesNotExist(productId);
            return;
        }

        if (product.GetValueOrThrow().IsClosed)
        {
            output.ProductAlreadyFreezed(productId);
            return;
        }

        var closedProduct = product.GetValueOrThrow().Close();
        await _productDao.CloseProductAsync(closedProduct);
        output.ProductFreezed(closedProduct.Id);
    }
}