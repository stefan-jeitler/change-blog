using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Queries.SharedModels;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogs
{
    public interface IGetPendingChangeLogLine
    {
        Task<Maybe<PendingChangeLogLineResponseModel>> ExecuteAsync(Guid userId, Guid changeLogLineId);
    }
}