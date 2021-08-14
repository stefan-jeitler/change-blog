using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Commands.UpdateChangeLogLine
{
    public interface IUpdateChangeLogLine
    {
        Task ExecuteAsync(IUpdateChangeLogLineOutputPort output, UpdateChangeLogLineRequestModel requestModel);
    }
}
