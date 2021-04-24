using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Command.AddVersion
{
    public interface IAddVersion
    {
        Task ExecuteAsync(IAddVersionOutputPort output, VersionRequestModel versionRequestModel);
    }
}