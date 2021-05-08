using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.AddCompleteVersion.Models;

namespace ChangeTracker.Application.UseCases.Commands.AddCompleteVersion
{
    public interface IAddCompleteVersion
    {
        Task ExecuteAsync(IAddCompleteVersionOutputPort output, CompleteVersionRequestModel versionRequestModel);
    }
}