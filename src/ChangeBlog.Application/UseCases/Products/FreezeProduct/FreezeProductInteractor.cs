using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Products;

namespace ChangeBlog.Application.UseCases.Products.FreezeProduct;

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

        if (product.GetValueOrThrow().IsFreezed)
        {
            output.ProductAlreadyFreezed(productId);
            return;
        }

        var freezedProduct = product.GetValueOrThrow().Freeze();
        await _productDao.FreezeProductAsync(freezedProduct);
        output.ProductFreezed(freezedProduct.Id);
    }
}