using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddVersion
{
    public interface IAddVersion
    {
        Task ExecuteAsync(IAddVersionOutputPort output, VersionRequestModel versionRequestModel);
    }
}