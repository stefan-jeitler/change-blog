using System;
using System.Text.Json.Serialization;

namespace ChangeBlog.Api.Shared.DTOs;

public class ErrorMessages
{
    public ErrorMessages(string message, string property)
    {
        ArgumentNullException.ThrowIfNull(message);

        Messages = new[] {message};
        Property = property.FirstCharToLower();
    }


    [JsonConstructor]
    public ErrorMessages(string[] messages, string property)
    {
        ArgumentNullException.ThrowIfNull(messages);

        Messages = messages;
        Property = property.FirstCharToLower();
    }

    public string[] Messages { get; }

    public string Property { get; }
}