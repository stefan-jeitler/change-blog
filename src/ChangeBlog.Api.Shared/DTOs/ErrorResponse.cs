using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ChangeBlog.Api.Shared.DTOs;

public class ErrorResponse
{
    public ErrorResponse(string message, IReadOnlyDictionary<string, string> resourceIds = null)
        : this(message, null, resourceIds)
    {
    }

    public ErrorResponse(string message, string property, IReadOnlyDictionary<string, string> resourceIds = null)
        : this(new[] {new ErrorMessage(message, property)}, resourceIds)
    {
    }

    public ErrorResponse(IEnumerable<ErrorMessage> responseMessages,
        IReadOnlyDictionary<string, string> resourceIds = null)
    {
        ArgumentNullException.ThrowIfNull(responseMessages);

        Errors = responseMessages.ToArray();
        ResourceIds = resourceIds;
    }

    public ErrorMessage[] Errors { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, string> ResourceIds { get; }

    public static ErrorResponse Create(string message)
    {
        return new ErrorResponse(message);
    }

    public static ErrorResponse Create(string message, string property)
    {
        return new ErrorResponse(message, property);
    }

    public static ErrorResponse Create(string message, IReadOnlyDictionary<string, string> resourceIds)
    {
        return new ErrorResponse(message, resourceIds);
    }
}