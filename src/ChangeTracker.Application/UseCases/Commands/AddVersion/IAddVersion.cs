using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.SharedModels;

namespace ChangeTracker.Application.UseCases.Commands.AddVersion
{
    public interface IAddVersion
    {
        Task ExecuteAsync(IAddVersionOutputPort output, VersionRequestModel requestModel);
    }
}