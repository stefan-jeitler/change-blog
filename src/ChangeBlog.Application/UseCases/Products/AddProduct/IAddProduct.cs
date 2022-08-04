using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Products.AddProduct;

public interface IAddProduct
{
    public Task ExecuteAsync(IAddProductOutputPort output, ProductRequestModel productRequestModel);
}