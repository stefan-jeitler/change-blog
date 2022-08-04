using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Users.DeleteApiKey;

public interface IDeleteApiKey
{
    Task ExecuteAsync(Guid userId, Guid apiKeyId);
}