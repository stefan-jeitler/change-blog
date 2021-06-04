using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Queries.SharedModels;

namespace ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogs
{
    public interface IGetPendingChangeLogLines
    {
        Task<PendingChangeLogsResponseModel> ExecuteAsync(Guid userId, Guid productId);
    }
}