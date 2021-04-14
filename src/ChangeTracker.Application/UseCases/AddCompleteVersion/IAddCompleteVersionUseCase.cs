using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.AddCompleteVersion.DTOs;

namespace ChangeTracker.Application.UseCases.AddCompleteVersion
{
    public interface IAddCompleteVersionUseCase
    {
        Task ExecuteAsync(IAddCompleteVersionOutputPort output, CompleteVersionDto versionDto);
    }
}