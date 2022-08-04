using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.ChangeLogs.GetPendingChangeLogs;

public interface IGetPendingChangeLogs
{
    Task<PendingChangeLogsResponseModel> ExecuteAsync(Guid userId, Guid productId);
}