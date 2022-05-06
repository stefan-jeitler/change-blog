using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ChangeBlog.Api.Shared.DTOs;

public class SuccessResponse
{   
    public SuccessResponse(string message, IReadOnlyDictionary<string, string> resourceIds = null)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message must not be empty.");
        
        Message = message;
        ResourceIds = resourceIds;
    }
    public string Message { get;}
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, string> ResourceIds { get; }
    
    
    public static SuccessResponse Create(string message)
    {
        return new SuccessResponse(message);
    }
    
    public static SuccessResponse Create(string message, IReadOnlyDictionary<string, string> resourceIds)
    {
        return new SuccessResponse(message, resourceIds);
    }
}