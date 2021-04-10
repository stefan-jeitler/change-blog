using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.CreateCompleteVersion.DTOs;

namespace ChangeTracker.Application.UseCases.CreateCompleteVersion
{
    public interface ICreateCompleteVersionUseCase
    {
        Task ExecuteAsync(ICreateCompleteVersionOutputPort output, CompleteVersionDto versionDto);
    }
}
