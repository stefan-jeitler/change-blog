using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Labels.SharedModels;

namespace ChangeTracker.Application.UseCases.Labels.AddChangeLogLineLabel
{
    public interface IAddChangeLogLineLabel
    {
        Task ExecuteAsync(IAddChangeLogLineLabelOutputPort output, ChangeLogLineLabelRequestModel requestModel);
    }
}