using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Products.ProductExists;

public interface IProductExists
{
    Task<bool> ExecuteAsync(Guid productId);
}