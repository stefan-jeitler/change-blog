using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public interface IAddChangeLogLine
    {
        Task ExecuteAsync(IAddChangeLogLineOutputPort output, ChangeLogLineRequestModel requestModel);
    }
}