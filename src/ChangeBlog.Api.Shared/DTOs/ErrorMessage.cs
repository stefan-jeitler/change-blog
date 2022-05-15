using System;
using System.Text.Json.Serialization;

namespace ChangeBlog.Api.Shared.DTOs;

public class ErrorMessage
{
    public ErrorMessage(string message, string property)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("message must not be empty.");
        
        Message = message;
        Property = property;
    }

    public string Message { get; }
    
    public string Property { get;}
}