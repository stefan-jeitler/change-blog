using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public interface IAddPendingChangeLogLine
    {
        Task ExecuteAsync(IAddPendingChangeLogLineOutputPort outputPort, PendingLineRequestModel lineRequestModel);
    }
}