using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Users.AddApiKey;

public interface IAddApiKeyOutputPort
{
    void ExpirationTooShort(DateTimeOffset expiresAt, TimeSpan minExpiration);
    void ExpirationTooLong(DateTimeOffset expiresAt, TimeSpan maxExpiration);
    void Conflict(Conflict conflict);
    void ApiKeyAdded(Guid apiKeyId);
    void MaxApiKeyCountReached(ushort maxApiKeys);
    void ExpirationDateInThePast(DateTime expiresAt);
}