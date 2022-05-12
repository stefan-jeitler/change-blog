using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.AddApiKey;

public interface IAddApiKeyOutputPort
{
    void ExpirationTooShort(TimeSpan expiresIn, TimeSpan minExpiration);
    void ExpirationTooLong(TimeSpan expiresIn, TimeSpan maxExpiration);
    void Conflict(Conflict conflict);
    void ApiKeyAdded(Guid apiKeyId);
    void InvalidTitle(string title);
    void MaxApiKeyCountReached(ushort maxApiKeys);
    void ExpirationDateInThePast(DateTime expiresAt);
}