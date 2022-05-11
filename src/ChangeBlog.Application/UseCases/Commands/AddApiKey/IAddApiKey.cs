using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Commands.AddApiKey;

public interface IAddApiKey
{
    Task ExecuteAsync(IAddApiKeyOutputPort output, AddApiKeyRequestModel requestModel);
}