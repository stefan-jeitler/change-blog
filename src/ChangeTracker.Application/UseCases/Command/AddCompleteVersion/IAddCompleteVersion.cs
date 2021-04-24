using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Command.AddCompleteVersion.Models;

namespace ChangeTracker.Application.UseCases.Command.AddCompleteVersion
{
    public interface IAddCompleteVersion
    {
        Task ExecuteAsync(IAddCompleteVersionOutputPort output, CompleteVersionRequestModel versionRequestModel);
    }
}