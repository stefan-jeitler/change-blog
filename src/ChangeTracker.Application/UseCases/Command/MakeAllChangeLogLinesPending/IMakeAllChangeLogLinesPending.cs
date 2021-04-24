using System;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Command.MakeAllChangeLogLinesPending
{
    public interface IMakeAllChangeLogLinesPending
    {
        Task ExecuteAsync(IMakeAllChangeLogLinesPendingOutputPort output, Guid versionId);
        Task ExecuteAsync(IMakeAllChangeLogLinesPendingOutputPort output, Guid projectId, string version);
    }
}