using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.ChangeLogs.Labels.SharedModels;

namespace ChangeBlog.Application.UseCases.ChangeLogs.Labels.AddChangeLogLineLabel;

public interface IAddChangeLogLineLabel
{
    Task ExecuteAsync(IAddChangeLogLineLabelOutputPort output, ChangeLogLineLabelRequestModel requestModel);
}