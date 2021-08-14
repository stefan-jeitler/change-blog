using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Commands.AddProduct
{
    public interface IAddProduct
    {
        public Task ExecuteAsync(IAddProductOutputPort output, ProductRequestModel productRequestModel);
    }
}
