using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Command.AddProject
{
    public interface IAddProject
    {
        public Task ExecuteAsync(IAddProjectOutputPort output, ProjectRequestModel projectRequestModel);
    }
}