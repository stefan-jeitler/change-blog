using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.CreateVersion
{
    public interface ICreateVersionUseCase
    {
        Task ExecuteAsync(ICreateVersionOutputPort output, VersionDto versionDto);
    }
}