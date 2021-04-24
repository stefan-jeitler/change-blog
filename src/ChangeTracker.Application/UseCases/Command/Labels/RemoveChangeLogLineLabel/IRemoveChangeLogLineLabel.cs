using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Command.Labels.SharedModels;

namespace ChangeTracker.Application.UseCases.Command.Labels.RemoveChangeLogLineLabel
{
    public interface IRemoveChangeLogLineLabel
    {
        Task ExecuteAsync(IRemoveChangeLogLineLabelOutputPort output, ChangeLogLineLabelRequestModel requestModel);
    }
}