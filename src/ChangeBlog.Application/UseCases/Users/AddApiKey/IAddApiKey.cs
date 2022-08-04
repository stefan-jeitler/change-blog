using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Users.AddApiKey;

public interface IAddApiKey
{
    Task ExecuteAsync(IAddApiKeyOutputPort output, AddApiKeyRequestModel requestModel);
}