using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Commands.DeleteChangeLogLine
{
    public interface IDeleteChangeLogLine
    {
        Task ExecuteAsync(IDeleteChangeLogLineOutputPort output, DeleteChangeLogLineRequestModel requestModel);
    }
}
