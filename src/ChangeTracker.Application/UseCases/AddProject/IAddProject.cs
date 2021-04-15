using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddProject
{
    public interface IAddProject
    {
        public Task ExecuteAsync(IAddProjectOutputPort output, ProjectRequestModel projectRequestModel);
    }
}