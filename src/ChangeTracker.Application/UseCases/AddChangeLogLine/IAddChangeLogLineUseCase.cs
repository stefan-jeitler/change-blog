using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public interface IAddChangeLogLineUseCase
    {
        Task ExecuteAsync(IAddChangeLogLineOutputPort output, ChangeLogLineDto changeLogLineDto);
    }
}