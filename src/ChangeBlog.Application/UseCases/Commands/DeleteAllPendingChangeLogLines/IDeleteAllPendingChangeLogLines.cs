using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Commands.DeleteAllPendingChangeLogLines
{
    public interface IDeleteAllPendingChangeLogLines
    {
        Task ExecuteAsync(Guid productId);
    }
}
