using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.Labels.SharedModels;

namespace ChangeTracker.Application.UseCases.Commands.Labels.AddChangeLogLineLabel
{
    public interface IAddChangeLogLineLabel
    {
        Task ExecuteAsync(IAddChangeLogLineLabelOutputPort output, ChangeLogLineLabelRequestModel requestModel);
    }
}