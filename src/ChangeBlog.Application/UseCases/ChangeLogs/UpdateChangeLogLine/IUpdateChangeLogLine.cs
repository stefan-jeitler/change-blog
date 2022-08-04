using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.ChangeLogs.UpdateChangeLogLine;

public interface IUpdateChangeLogLine
{
    Task ExecuteAsync(IUpdateChangeLogLineOutputPort output, UpdateChangeLogLineRequestModel requestModel);
}