using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.AddCompleteVersion.Models;

namespace ChangeTracker.Application.UseCases.AddCompleteVersion
{
    public interface IAddCompleteVersion
    {
        Task ExecuteAsync(IAddCompleteVersionOutputPort output, CompleteVersionRequestModel versionRequestModel);
    }
}