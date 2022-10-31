using System;
using ChangeBlog.Application.UseCases.Users.GetApiKeys;

namespace ChangeBlog.Management.Api.DTOs.V1.ApiKey;

public class ApiKeyDto
{
    public Guid ApiKeyId { get; set; }
    public string ApiKey { get; set; }
    public string Name { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }

    public static ApiKeyDto FromResponseModel(ApiKeyResponseModel m) =>
        new()
        {
            ApiKeyId = m.ApiKeyId,
            Name = m.Name,
            ApiKey = m.ApiKey,
            ExpiresAt = m.ExpiresAt
        };
}