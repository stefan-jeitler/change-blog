using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Commands.CloseProduct
{
    public interface ICloseProduct
    {
        Task ExecuteAsync(ICloseProductOutputPort output, Guid productId);
    }
}
