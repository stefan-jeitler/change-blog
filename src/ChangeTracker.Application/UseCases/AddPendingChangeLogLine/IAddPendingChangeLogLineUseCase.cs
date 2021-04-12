using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public interface IAddPendingChangeLogLineUseCase
    {
        Task ExecuteAsync(IAddPendingChangeLogLineOutputPort outputPort, PendingChangeLogLineDto changeLogLineDto);
    }
}