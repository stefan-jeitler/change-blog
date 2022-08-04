using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Users.UpdateUserProfile;

public interface IUpdateUserProfileOutputPort
{
    void Updated(Guid userId);
    void TimezoneNotFound(string timezone);
    void CultureNotFound(string culture);
    void Conflict(Conflict conflict);
}