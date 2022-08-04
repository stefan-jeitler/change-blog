using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.ChangeLogs.AddPendingChangeLogLine;

public interface IAddPendingChangeLogLine
{
    Task ExecuteAsync(IAddPendingChangeLogLineOutputPort outputPort,
        PendingChangeLogLineRequestModel lineRequestModel);
}