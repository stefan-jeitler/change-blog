using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.SharedModels;

namespace ChangeTracker.Application.UseCases.Commands.AddProduct
{
    public interface IAddProduct
    {
        public Task ExecuteAsync(IAddProductOutputPort output, ProductRequestModel productRequestModel);
    }
}