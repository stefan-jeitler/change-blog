using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Commands.MakeChangeLogLinePending
{
    public interface IMakeChangeLogLinePending
    {
        Task ExecuteAsync(IMakeChangeLogLinePendingOutputPort output, Guid changeLogLineId);
    }
}
