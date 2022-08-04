using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.ChangeLogs.MakeAllChangeLogLinesPending;

public interface IMakeAllChangeLogLinesPending
{
    Task ExecuteAsync(IMakeAllChangeLogLinesPendingOutputPort output, Guid versionId);
    Task ExecuteAsync(IMakeAllChangeLogLinesPendingOutputPort output, Guid productId, string version);
}