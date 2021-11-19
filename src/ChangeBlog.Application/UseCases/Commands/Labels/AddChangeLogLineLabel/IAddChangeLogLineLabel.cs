using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Commands.Labels.SharedModels;

namespace ChangeBlog.Application.UseCases.Commands.Labels.AddChangeLogLineLabel;

public interface IAddChangeLogLineLabel
{
    Task ExecuteAsync(IAddChangeLogLineLabelOutputPort output, ChangeLogLineLabelRequestModel requestModel);
}