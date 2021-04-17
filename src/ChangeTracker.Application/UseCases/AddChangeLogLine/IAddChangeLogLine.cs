using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public interface IAddChangeLogLine
    {
        Task ExecuteAsync(IAddLineOutputPort output, ChangeLogLineRequestModel requestModel);
    }
}