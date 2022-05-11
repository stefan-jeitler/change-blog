using System;

namespace ChangeBlog.Application.UseCases.Queries.GetApiKeys;

public class ApiKeyResponseModel
{
    public ApiKeyResponseModel(Guid apiKeyId, string title, string apiKey, DateTimeOffset expiresAt)
    {
        if (apiKeyId == Guid.Empty)
            throw new ArgumentException("apiKeyId must not be empty.");

        ApiKeyId = apiKeyId;
        
        ArgumentNullException.ThrowIfNull(title);

        Title = title;
        
        ArgumentNullException.ThrowIfNull(apiKey);

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("apiKey must not be empty.");
        
        ApiKey = apiKey;

        if (expiresAt == DateTimeOffset.MinValue || expiresAt == DateTimeOffset.MaxValue)
            throw new ArgumentException("expiresAt must not be min or max value.");
        
        ExpiresAt = expiresAt;
    }

    public Guid ApiKeyId { get;}
    public string Title { get; set; }
    public string ApiKey { get; }
    public DateTimeOffset ExpiresAt { get; }
}