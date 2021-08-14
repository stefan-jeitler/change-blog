using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Commands.AddPendingChangeLogLine
{
    public interface IAddPendingChangeLogLine
    {
        Task ExecuteAsync(IAddPendingChangeLogLineOutputPort outputPort,
            PendingChangeLogLineRequestModel lineRequestModel);
    }
}
