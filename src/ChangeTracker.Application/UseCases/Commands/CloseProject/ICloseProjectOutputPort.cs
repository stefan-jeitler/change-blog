using System;

namespace ChangeTracker.Application.UseCases.Commands.CloseProject
{
    public interface ICloseProjectOutputPort
    {
        void ProjectAlreadyClosed();
        void ProjectDoesNotExist();
        void Conflict(string reason);
        void ProjectClosed(Guid projectId);
    }
}