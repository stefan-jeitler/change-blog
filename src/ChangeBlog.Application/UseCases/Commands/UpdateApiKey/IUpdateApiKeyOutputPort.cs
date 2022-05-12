using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.UpdateApiKey;

public interface IUpdateApiKeyOutputPort
{
    void ApiKeyNotFound(Guid apiKeyId);
    void ExpirationTooShort(TimeSpan expiresIn, TimeSpan minExpiration);
    void ExpirationTooLong(TimeSpan expiresIn, TimeSpan maxExpiration);
    void ExpirationDateInThePast(DateTime expiresAt);
    void InvalidTitle(string title);
    void Conflict(Conflict conflict);
    void Updated(Guid apiKeyId);
}