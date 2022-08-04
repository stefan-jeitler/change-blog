using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.ChangeLogs.DeleteChangeLogLine;

public interface IDeleteChangeLogLine
{
    Task ExecuteAsync(IDeleteChangeLogLineOutputPort output, DeleteChangeLogLineRequestModel requestModel);
}