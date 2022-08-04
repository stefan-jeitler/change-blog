using System;

namespace ChangeBlog.Application.UseCases.Products.CloseProduct;

public interface ICloseProductOutputPort
{
    void ProductAlreadyClosed(Guid productId);
    void ProductDoesNotExist(Guid productId);
    void ProductClosed(Guid productId);
}