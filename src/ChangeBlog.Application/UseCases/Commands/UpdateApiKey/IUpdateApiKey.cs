using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Commands.UpdateApiKey;

public interface IUpdateApiKey
{
    Task ExecuteAsync(IUpdateApiKeyOutputPort output, UpdateApiKeyRequestModel requestModel);
}