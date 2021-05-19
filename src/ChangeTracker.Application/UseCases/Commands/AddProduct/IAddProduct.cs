using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.AddProduct
{
    public interface IAddProduct
    {
        public Task ExecuteAsync(IAddProductOutputPort output, ProductRequestModel productRequestModel);
    }
}