using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.ChangeLogs.Labels.SharedModels;

namespace ChangeBlog.Application.UseCases.ChangeLogs.Labels.DeleteChangeLogLineLabel;

public interface IDeleteChangeLogLineLabel
{
    Task ExecuteAsync(IDeleteChangeLogLineLabelOutputPort output, ChangeLogLineLabelRequestModel requestModel);
}