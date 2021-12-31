using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChangeBlog.Api.Shared.DTOs;

public class DefaultResponse
{
    public DefaultResponse(string message, IReadOnlyDictionary<string, string> resourceIds = null)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty.");
        }

        Message = message;

        ResourceIds = resourceIds;
    }

    public string Message { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, string> ResourceIds { get; }

    public static DefaultResponse Create(string message)
    {
        return new DefaultResponse(message);
    }

    public static DefaultResponse Create(string message, IReadOnlyDictionary<string, string> resourceIds)
    {
        return new DefaultResponse(message, resourceIds);
    }
}