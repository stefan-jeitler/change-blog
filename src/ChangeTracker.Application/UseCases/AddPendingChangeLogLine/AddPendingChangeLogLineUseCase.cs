using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public class AddPendingChangeLogLineUseCase : IAddPendingChangeLogLineUseCase
    {
        public Task ExecuteAsync(IAddPendingChangeLogLineOutput output, AddPendingChangeLogLineDto changeLogLineDto)
        {
            return Task.CompletedTask;
        }
    }
}
