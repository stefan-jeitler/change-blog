﻿using System;
using ChangeBlog.Application.UseCases.Queries.GetUsers;

namespace ChangeBlog.Management.Api.DTOs;

public class UserDto
{
    public UserDto(Guid id, string email, string firstName, string lastName, string timeZone,
        DateTimeOffset createdAt)
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
    public DateTimeOffset CreatedAt { get; }

    public static UserDto FromResponseModel(UserResponseModel m)
    {
        return new UserDto(
            m.Id,
            m.Email,
            m.FirstName,
            m.LastName,
            m.TimeZone,
            m.CreatedAt
        );
    }
}