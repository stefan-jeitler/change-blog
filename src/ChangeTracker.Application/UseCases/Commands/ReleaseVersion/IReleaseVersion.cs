using System;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.ReleaseVersion
{
    public interface IReleaseVersion
    {
        Task ExecuteAsync(IReleaseVersionOutputPort output, Guid versionId);
    }
}