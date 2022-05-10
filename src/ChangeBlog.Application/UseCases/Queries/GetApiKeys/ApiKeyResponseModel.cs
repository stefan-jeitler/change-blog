using System;

namespace ChangeBlog.Application.UseCases.Queries.GetApiKeys;

public class ApiKeyResponseModel
{
    public ApiKeyResponseModel(Guid apiKeyId, string apiKey, DateTimeOffset expiresAt)
    {
        if (apiKeyId == Guid.Empty)
            throw new ArgumentException("apiKeyId must not be empty.");

        ApiKeyId = apiKeyId;
        
        ArgumentNullException.ThrowIfNull(apiKey);

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("apiKey must not be empty.");
        
        ApiKey = apiKey;

        if (expiresAt == DateTimeOffset.MinValue || expiresAt == DateTimeOffset.MaxValue)
            throw new ArgumentException("expiresAt must not be min or max value.");
        
        ExpiresAt = expiresAt;
    }

    public Guid ApiKeyId { get;}
    public string ApiKey { get; }
    public DateTimeOffset ExpiresAt { get; }
}