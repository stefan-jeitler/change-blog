using System;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.MakeChangeLogLinePending
{
    public interface IMakeChangeLogLinePending
    {
        Task ExecuteAsync(IMakeChangeLogLinePendingOutputPort output, Guid changeLogLineId);
    }
}