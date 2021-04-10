using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public interface IAddPendingChangeLogLineUseCase
    {
        Task ExecuteAsync(IAddPendingChangeLogLineOutputPort outputPort, PendingChangeLogLineDto changeLogLineDto);
    }
}
