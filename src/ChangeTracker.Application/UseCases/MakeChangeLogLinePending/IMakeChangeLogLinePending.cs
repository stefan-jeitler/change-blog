using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.MakeChangeLogLinePending
{
    public interface IMakeChangeLogLinePending
    {
        Task ExecuteAsync(IMakeChangeLogLinePendingOutputPort output, Guid changeLogLineId);
    }
}
