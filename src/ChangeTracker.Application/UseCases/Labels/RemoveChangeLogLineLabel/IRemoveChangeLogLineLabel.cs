using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Labels.SharedModels;

namespace ChangeTracker.Application.UseCases.Labels.RemoveChangeLogLineLabel
{
    public interface IRemoveChangeLogLineLabel
    {
        Task ExecuteAsync(IRemoveChangeLogLineLabelOutputPort output, ChangeLogLineLabelRequestModel requestModel);
    }
}