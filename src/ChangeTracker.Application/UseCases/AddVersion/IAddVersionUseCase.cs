using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddVersion
{
    public interface IAddVersionUseCase
    {
        Task ExecuteAsync(IAddVersionOutputPort output, VersionDto versionDto);
    }
}