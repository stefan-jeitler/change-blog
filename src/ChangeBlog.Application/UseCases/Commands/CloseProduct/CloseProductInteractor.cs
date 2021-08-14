using System;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess.Products;

namespace ChangeBlog.Application.UseCases.Commands.CloseProduct
{
    public class CloseProductInteractor : ICloseProduct
    {
        private readonly IProductDao _productDao;

        public CloseProductInteractor(IProductDao productDao)
        {
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
        }

        public async Task ExecuteAsync(ICloseProductOutputPort output, Guid productId)
        {
            var product = await _productDao.FindProductAsync(productId);
            if (product.HasNoValue)
            {
                output.ProductDoesNotExist(productId);
                return;
            }

            if (product.Value.IsClosed)
            {
                output.ProductAlreadyClosed(productId);
                return;
            }

            var closedProduct = product.Value.Close();
            await _productDao.CloseProductAsync(closedProduct);
            output.ProductClosed(closedProduct.Id);
        }
    }
}
