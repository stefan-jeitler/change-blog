using System;
using System.Text.Json.Serialization;

namespace ChangeBlog.Api.Shared.DTOs;

public class PropertyErrorMessages
{
    public PropertyErrorMessages(string message, string property)
    {
        ArgumentNullException.ThrowIfNull(message);

        Messages = new[] {message};
        Property = property;
    }


    [JsonConstructor]
    public PropertyErrorMessages(string[] messages, string property)
    {
        ArgumentNullException.ThrowIfNull(messages);

        Messages = messages;
        Property = property;
    }

    public string[] Messages { get; }

    public string Property { get; }
}