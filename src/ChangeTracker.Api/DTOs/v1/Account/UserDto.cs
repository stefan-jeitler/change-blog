using System;
using ChangeTracker.Application.UseCases.Queries.GetUsers;

namespace ChangeTracker.Api.DTOs.v1.Account
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TimeZone { get; set; }
        public DateTime CreatedAt { get; set; }

        public static UserDto FromResponseModel(UserResponseModel m) =>
            new()
            {
                Id = m.Id,
                Email = m.Email,
                FirstName = m.FirstName,
                LastName = m.LastName,
                TimeZone = m.TimeZone,
                CreatedAt = m.CreatedAt
            };
    }
}