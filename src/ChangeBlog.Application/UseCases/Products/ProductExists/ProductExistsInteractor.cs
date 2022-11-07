using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Products;

namespace ChangeBlog.Application.UseCases.Products.ProductExists;

public class ProductExistsInteractor : IProductExists
{
    private readonly IProductDao _productDao;

    public ProductExistsInteractor(IProductDao productDao)
    {
        _productDao = productDao;
    }

    public async Task<bool> ExecuteAsync(Guid productId)
    {
        var product = await _productDao.FindProductAsync(productId);

        return product.HasValue;
    }
}