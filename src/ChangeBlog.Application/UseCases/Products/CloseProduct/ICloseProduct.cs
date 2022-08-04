using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Products.CloseProduct;

public interface ICloseProduct
{
    Task ExecuteAsync(ICloseProductOutputPort output, Guid productId);
}