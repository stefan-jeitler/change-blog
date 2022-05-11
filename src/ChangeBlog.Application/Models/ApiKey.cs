using System;

namespace ChangeBlog.Application.Models;

public class ApiKey
{
    public ApiKey(Guid userId, Guid apiKeyId, string title, string apiKey, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("userId must not be empty.");
        
        UserId = userId;

        if (apiKeyId == Guid.Empty)
            throw new ArgumentException("apiKeyId must not be empty.");

        ApiKeyId = apiKeyId;

        Title = title ?? string.Empty;
        
        ArgumentNullException.ThrowIfNull(apiKey);

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("apiKey must not be empty.");
        
        Key = apiKey;

        if (expiresAt == DateTime.MinValue || expiresAt == DateTime.MaxValue)
            throw new ArgumentException("expiresAt must not be min or max value.");
        
        ExpiresAt = expiresAt;
    }

    public Guid UserId { get; }
    public Guid ApiKeyId { get; }
    public string Title { get; }
    public string Key { get; }
    public DateTime ExpiresAt { get; }
}