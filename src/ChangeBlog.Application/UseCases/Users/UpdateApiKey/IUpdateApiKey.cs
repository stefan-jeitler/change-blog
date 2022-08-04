using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Users.UpdateApiKey;

public interface IUpdateApiKey
{
    Task ExecuteAsync(IUpdateApiKeyOutputPort output, UpdateApiKeyRequestModel requestModel);
}