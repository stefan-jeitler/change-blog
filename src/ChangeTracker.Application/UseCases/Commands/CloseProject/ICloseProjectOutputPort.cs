using System;

namespace ChangeTracker.Application.UseCases.Commands.CloseProject
{
    public interface ICloseProjectOutputPort
    {
        void ProjectAlreadyClosed(Guid projectId);
        void ProjectDoesNotExist();
        void ProjectClosed(Guid projectId);
    }
}