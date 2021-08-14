using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Queries.GetPendingChangeLogs
{
    public interface IGetPendingChangeLogs
    {
        Task<PendingChangeLogsResponseModel> ExecuteAsync(Guid userId, Guid productId);
    }
}
