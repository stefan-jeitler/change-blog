using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.Labels.SharedModels;

namespace ChangeTracker.Application.UseCases.Commands.Labels.RemoveChangeLogLineLabel
{
    public interface IRemoveChangeLogLineLabel
    {
        Task ExecuteAsync(IRemoveChangeLogLineLabelOutputPort output, ChangeLogLineLabelRequestModel requestModel);
    }
}