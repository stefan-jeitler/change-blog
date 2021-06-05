using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.Labels.SharedModels;

namespace ChangeTracker.Application.UseCases.Commands.Labels.DeleteChangeLogLineLabel
{
    public interface IDeleteChangeLogLineLabel
    {
        Task ExecuteAsync(IDeleteChangeLogLineLabelOutputPort output, ChangeLogLineLabelRequestModel requestModel);
    }
}