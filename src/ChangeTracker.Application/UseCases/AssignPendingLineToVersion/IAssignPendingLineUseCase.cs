using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.AssignPendingLineToVersion.DTOs;

namespace ChangeTracker.Application.UseCases.AssignPendingLineToVersion
{
    public interface IAssignPendingLineUseCase
    {
        Task ExecuteAsync(IAssignPendingLineOutputPort output, PendingLineByVersionIdDto assignmentDto);
        Task ExecuteAsync(IAssignPendingLineOutputPort output, PendingLineByVersionDto assignmentDto);
    }
}
