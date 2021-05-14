using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Projects;

namespace ChangeTracker.Application.UseCases.Commands.CloseProject
{
    public class CloseProjectInteractor : ICloseProject
    {
        private readonly IProjectDao _projectDao;

        public CloseProjectInteractor(IProjectDao projectDao)
        {
            _projectDao = projectDao;
        }

        public async Task ExecuteAsync(ICloseProjectOutputPort output, Guid projectId)
        {
            var project = await _projectDao.FindProjectAsync(projectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return;
            }

            if (project.Value.IsClosed)
            {
                output.ProjectAlreadyClosed(projectId);
                return;
            }

            var closedProject = project.Value.Close();
            await _projectDao.CloseProjectAsync(closedProject);
            output.ProjectClosed(closedProject.Id);
        }
    }
}