using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Products.CloseProduct;

public interface IFreezeProduct
{
    Task ExecuteAsync(IFreezeProductOutputPort output, Guid productId);
}