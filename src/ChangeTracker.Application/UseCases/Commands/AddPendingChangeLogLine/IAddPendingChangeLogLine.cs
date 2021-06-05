using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.AddPendingChangeLogLine
{
    public interface IAddPendingChangeLogLine
    {
        Task ExecuteAsync(IAddPendingChangeLogLineOutputPort outputPort, PendingChangeLogLineRequestModel lineRequestModel);
    }
}