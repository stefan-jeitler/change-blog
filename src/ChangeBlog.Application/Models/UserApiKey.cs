using System;

namespace ChangeBlog.Application.Models;

public class UserApiKey
{
    public UserApiKey(Guid userId, Guid apiKeyId, string apiKey, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("userId must not be empty.");
        
        UserId = userId;

        if (apiKeyId == Guid.Empty)
            throw new ArgumentException("apiKeyId must not be empty.");

        ApiKeyId = apiKeyId;
        
        ArgumentNullException.ThrowIfNull(apiKey);

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("apiKey must not be empty.");
        
        ApiKey = apiKey;

        if (expiresAt == DateTime.MinValue || expiresAt == DateTime.MaxValue)
            throw new ArgumentException("expiresAt must not be min or max value.");
        
        ExpiresAt = expiresAt;
    }

    public Guid UserId { get; }
    public Guid ApiKeyId { get; }
    public string ApiKey { get; }
    public DateTime ExpiresAt { get; }
}