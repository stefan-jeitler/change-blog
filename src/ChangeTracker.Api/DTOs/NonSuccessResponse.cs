using System;

namespace ChangeTracker.Api.DTOs
{
    public class NonSuccessResponse
    {
        public NonSuccessResponse(string key, string message)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.");

            Key = key;

            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Message cannot be null or empty.");
            
            Message = message;
        }

        public string Key { get; }
        public string Message { get; }
        public static NonSuccessResponse Create(string key, string message) => new(key, message);
    }
}