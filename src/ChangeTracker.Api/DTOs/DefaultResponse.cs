using System;
using System.Text.Json.Serialization;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ChangeTracker.Api.DTOs
{
    public class DefaultResponse
    {
        public DefaultResponse(string message, Guid? resourceId = null)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Message cannot be null or empty.");

            Message = message;

            ResourceId = resourceId;
        }

        public string Message { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? ResourceId { get; }

        public static DefaultResponse Create(string message) => new(message);
        public static DefaultResponse Create(string message, Guid resourceId) => new(message, resourceId);
    }
}