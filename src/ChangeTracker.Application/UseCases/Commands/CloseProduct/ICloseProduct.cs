using System;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.CloseProduct
{
    public interface ICloseProduct
    {
        Task ExecuteAsync(ICloseProductOutputPort output, Guid productId);
    }
}