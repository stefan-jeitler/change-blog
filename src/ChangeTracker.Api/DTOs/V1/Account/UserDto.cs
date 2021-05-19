using System;
using ChangeTracker.Application.UseCases.Queries.GetUsers;

namespace ChangeTracker.Api.DTOs.V1.Account
{
    public class UserDto
    {
        public UserDto(Guid id, string email, string firstName, string lastName, string timeZone, DateTime createdAt)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            TimeZone = timeZone;
            CreatedAt = createdAt;
        }

        public Guid Id { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string TimeZone { get; }
        public DateTime CreatedAt { get; }

        public static UserDto FromResponseModel(UserResponseModel m)
        {
            return new(
                m.Id,
                m.Email,
                m.FirstName,
                m.LastName,
                m.TimeZone,
                m.CreatedAt
            );
        }
    }
}