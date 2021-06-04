using System;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.DeleteAllPendingChangeLogLines
{
    public interface IDeleteAllPendingChangeLogLines
    {
        Task ExecuteAsync(Guid productId);
    }
}