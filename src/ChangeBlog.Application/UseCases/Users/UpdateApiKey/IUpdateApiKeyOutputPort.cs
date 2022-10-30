using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Users.UpdateApiKey;

public interface IUpdateApiKeyOutputPort
{
    void ApiKeyNotFound(Guid apiKeyId);
    void ExpirationTooShort(TimeSpan expiresIn, TimeSpan minExpiration);
    void ExpirationTooLong(TimeSpan expiresIn, TimeSpan maxExpiration);
    void ExpirationDateInThePast(DateTime expiresAt);
    void Conflict(Conflict conflict);
    void Updated(Guid apiKeyId);
}