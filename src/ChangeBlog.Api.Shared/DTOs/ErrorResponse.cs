using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChangeBlog.Api.Shared.DTOs;

public class ErrorResponse
{
    public ErrorResponse(string message, IReadOnlyDictionary<string, string> resourceIds = null)
        : this(message, null, resourceIds)
    {
    }

    public ErrorResponse(string message, string property, IReadOnlyDictionary<string, string> resourceIds = null)
        : this(new[] {new PropertyErrorMessages(message, property)}, resourceIds)
    {
    }

    [JsonConstructor]
    public ErrorResponse(PropertyErrorMessages[] errors,
        IReadOnlyDictionary<string, string> resourceIds = null)
    {
        ArgumentNullException.ThrowIfNull(errors);

        Errors = errors;
        ResourceIds = resourceIds;
    }

    public PropertyErrorMessages[] Errors { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, string> ResourceIds { get; }

    public static ErrorResponse Create(string message) => new(message);

    public static ErrorResponse Create(string message, string property) => new(message, property);

    public static ErrorResponse Create(string message, IReadOnlyDictionary<string, string> resourceIds) =>
        new(message, resourceIds);
}