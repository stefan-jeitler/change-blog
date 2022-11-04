using System;

namespace ChangeBlog.Application.UseCases.Products.FreezeProduct;

public interface IFreezeProductOutputPort
{
    void ProductAlreadyFreezed(Guid productId);
    void ProductDoesNotExist(Guid productId);
    void ProductFreezed(Guid productId);
}