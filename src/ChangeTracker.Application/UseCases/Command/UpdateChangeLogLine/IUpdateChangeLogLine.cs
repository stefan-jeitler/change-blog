using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Command.UpdateChangeLogLine
{
    public interface IUpdateChangeLogLine
    {
        Task ExecuteAsync(IUpdateLineOutputPort output, ChangeLogLineRequestModel requestModel);
    }
}