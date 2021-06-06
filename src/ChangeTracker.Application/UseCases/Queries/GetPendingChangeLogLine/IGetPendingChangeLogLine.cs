using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogLine
{
    public interface IGetPendingChangeLogLine
    {
        Task ExecuteAsync(IGetPendingChangeLogLineOutputPort output,
            Guid userId, Guid changeLogLineId);
    }
}