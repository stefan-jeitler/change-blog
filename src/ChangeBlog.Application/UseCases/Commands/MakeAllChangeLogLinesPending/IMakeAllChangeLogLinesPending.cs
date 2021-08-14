using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Commands.MakeAllChangeLogLinesPending
{
    public interface IMakeAllChangeLogLinesPending
    {
        Task ExecuteAsync(IMakeAllChangeLogLinesPendingOutputPort output, Guid versionId);
        Task ExecuteAsync(IMakeAllChangeLogLinesPendingOutputPort output, Guid productId, string version);
    }
}
