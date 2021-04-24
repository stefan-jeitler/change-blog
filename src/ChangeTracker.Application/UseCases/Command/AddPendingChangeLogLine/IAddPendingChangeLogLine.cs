using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Command.AddPendingChangeLogLine
{
    public interface IAddPendingChangeLogLine
    {
        Task ExecuteAsync(IAddPendingLineOutputPort outputPort, PendingLineRequestModel lineRequestModel);
    }
}