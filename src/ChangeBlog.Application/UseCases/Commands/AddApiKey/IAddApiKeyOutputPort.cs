using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.AddApiKey;

public interface IAddApiKeyOutputPort
{
    void ExpirationTooShort(string userCulture, TimeSpan expiresIn, TimeSpan minExpiration);
    void ExpirationTooLong(string userCulture, TimeSpan expiresIn, TimeSpan maxExpiration);
    void Conflict(Conflict conflict);
    void ApiKeyAdded(Guid apiKeyId);
}