namespace ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;

public record ExternalUserInfo(string Id,
    string Email,
    string FirstName,
    string LastName);