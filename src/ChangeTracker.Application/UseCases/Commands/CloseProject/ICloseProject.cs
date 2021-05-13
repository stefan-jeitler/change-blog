using System;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.CloseProject
{
    public interface ICloseProject
    {
        Task ExecuteAsync(ICloseProjectOutputPort output, Guid projectId);
    }
}