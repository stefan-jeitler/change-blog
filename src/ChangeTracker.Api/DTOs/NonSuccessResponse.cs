using System;

namespace ChangeTracker.Api.DTOs
{
    public class NonSuccessResponse
    {
        public NonSuccessResponse(string message)
        {

            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Message cannot be null or empty.");
            
            Message = message;
        }
        
        public string Message { get; }
        public static NonSuccessResponse Create(string message) => new(message);
    }
}