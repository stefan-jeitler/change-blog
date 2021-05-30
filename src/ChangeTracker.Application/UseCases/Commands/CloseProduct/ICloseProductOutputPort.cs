using System;

namespace ChangeTracker.Application.UseCases.Commands.CloseProduct
{
    public interface ICloseProductOutputPort
    {
        void ProductAlreadyClosed(Guid productId);
        void ProductDoesNotExist(Guid productId);
        void ProductClosed(Guid productId);
    }
}