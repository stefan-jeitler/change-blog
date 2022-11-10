using System;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.Users.GetApiKeys;

public class ApiKeyResponseModel
{
    public ApiKeyResponseModel(Guid apiKeyId, string name, string apiKey, DateTimeOffset expiresAt)
    {
        ApiKeyId = Guard.Against.NullOrEmpty(apiKeyId, nameof(apiKeyId));
        ArgumentNullException.ThrowIfNull(name);

        Name = name;

        ArgumentNullException.ThrowIfNull(apiKey);

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("apiKey must not be empty.");

        ApiKey = apiKey;

        if (expiresAt == DateTimeOffset.MinValue || expiresAt == DateTimeOffset.MaxValue)
            throw new ArgumentException("expiresAt must not be min or max value.");

        ExpiresAt = expiresAt;
    }

    public Guid ApiKeyId { get; }
    public string Name { get; }
    public string ApiKey { get; }
    public DateTimeOffset ExpiresAt { get; }
}