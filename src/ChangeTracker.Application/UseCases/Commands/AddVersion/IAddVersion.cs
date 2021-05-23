using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.AddVersion
{
    public interface IAddVersion
    {
        Task ExecuteAsync(IAddVersionOutputPort output, VersionRequestModel requestModel);
    }
}