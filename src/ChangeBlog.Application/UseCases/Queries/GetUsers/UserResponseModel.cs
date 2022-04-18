using System;

namespace ChangeBlog.Application.UseCases.Queries.GetUsers;

public class UserResponseModel
{
    public UserResponseModel(Guid id, string email, string firstName, string lastName, string timeZone,
        string culture, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        TimeZone = timeZone;
        Culture = culture;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string TimeZone { get; }
    public string Culture { get; }
    public DateTimeOffset CreatedAt { get; }
}