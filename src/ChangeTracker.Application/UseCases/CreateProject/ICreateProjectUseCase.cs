using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.CreateProject
{
    public interface ICreateProjectUseCase
    {
        public Task ExecuteAsync(ICreateProjectOutputPort output, ProjectDto projectDto);
    }
}