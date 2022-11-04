using System;

namespace ChangeBlog.Application.UseCases.Products.CloseProduct;

public interface IFreezeProductOutputPort
{
    void ProductAlreadyFreezed(Guid productId);
    void ProductDoesNotExist(Guid productId);
    void ProductFreezed(Guid productId);
}