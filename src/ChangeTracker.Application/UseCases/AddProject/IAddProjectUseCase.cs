using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddProject
{
    public interface IAddProjectUseCase
    {
        public Task ExecuteAsync(IAddProjectOutputPort output, ProjectDto projectDto);
    }
}