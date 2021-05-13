using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.CloseProject
{
    public interface ICloseProject
    {
        Task ExecuteAsync(ICloseProjectOutputPort output, Guid projectId);
    }
}
