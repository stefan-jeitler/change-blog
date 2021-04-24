using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Command.Labels.SharedModels;

namespace ChangeTracker.Application.UseCases.Command.Labels.AddChangeLogLineLabel
{
    public interface IAddChangeLogLineLabel
    {
        Task ExecuteAsync(IAddChangeLogLineLabelOutputPort output, ChangeLogLineLabelRequestModel requestModel);
    }
}