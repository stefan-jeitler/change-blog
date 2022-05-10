using System;
using ChangeBlog.Application.UseCases.Queries.GetApiKeys;

namespace ChangeBlog.Management.Api.DTOs.V1;

public class ApiKeyDto
{
    public Guid ApiKeyId { get; set; }
    public string ApiKey { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }

    public static ApiKeyDto FromResponseModel(ApiKeyResponseModel m)
    {
        return new ApiKeyDto
        {
            ApiKeyId = m.ApiKeyId,
            ApiKey = m.ApiKey,
            ExpiresAt = m.ExpiresAt
        };
    }
}