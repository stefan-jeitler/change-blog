using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Commands.Labels.SharedModels;

namespace ChangeBlog.Application.UseCases.Commands.Labels.DeleteChangeLogLineLabel;

public interface IDeleteChangeLogLineLabel
{
    Task ExecuteAsync(IDeleteChangeLogLineLabelOutputPort output, ChangeLogLineLabelRequestModel requestModel);
}