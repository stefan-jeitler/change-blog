using System;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ChangeTracker.Api.DTOs
{
    public class DefaultResponse
    {
        public DefaultResponse(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Message cannot be null or empty.");

            Message = message;
        }

        public string Message { get; }
        public static DefaultResponse Create(string message) => new(message);
    }
}