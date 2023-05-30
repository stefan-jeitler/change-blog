using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;
using NodaTime;
using NodaTime.TimeZones;

namespace ChangeBlog.Application.UseCases.Users.UpdateUserProfile;

public class UpdateUserProfileInteractor : IUpdateUserProfile
{
    private readonly IBusinessTransaction _businessTransaction;
    private readonly IUserDao _userDao;

    public UpdateUserProfileInteractor(IUserDao userDao, IBusinessTransaction businessTransaction)
    {
        _userDao = userDao;
        _businessTransaction = businessTransaction;
    }

    public async Task ExecuteAsync(IUpdateUserProfileOutputPort output,
        UpdateUserProfileRequestModel requestModel)
    {
        if (requestModel.UserId == Guid.Empty)
            throw new ArgumentException("Missing UserId.");

        var currentUser = await _userDao.GetUserAsync(requestModel.UserId);

        var (isSuccess, user) = UpdateTimezone(output, currentUser, requestModel.OlsonOrWindowsId)
            .Bind(x => UpdateCulture(output, x, requestModel.Culture));

        if (isSuccess)
        {
            await _userDao.UpdateCultureAndTimezoneAsync(user)
                .Match(Finish, output.Conflict);
        }

        void Finish(User updateUser)
        {
            _businessTransaction.Commit();
            output.Updated(user.Id);
        }
    }

    private static Maybe<User> UpdateTimezone(IUpdateUserProfileOutputPort output, User currentUser,
        string olsonOrWindowsId)
    {
        if (!Name.TryParse(olsonOrWindowsId, out var timezoneId))
        {
            return Maybe<User>.From(currentUser);
        }

        var timeZone = TzdbDateTimeZoneSource.Default.WindowsToTzdbIds.GetValueOrDefault(timezoneId.Value)
                       ?? DateTimeZoneProviders.Tzdb.GetZoneOrNull(timezoneId.Value)?.Id;

        if (timeZone is null)
        {
            output.TimezoneNotFound(timezoneId.Value);
            return Maybe<User>.None;
        }

        var updatedUser = currentUser.UpdateTimezone(Name.Parse(timeZone));
        return Maybe<User>.From(updatedUser);
    }

    private static Maybe<User> UpdateCulture(IUpdateUserProfileOutputPort output, User currentUser, string culture)
    {
        if (!Name.TryParse(culture, out var c))
        {
            return Maybe.From(currentUser);
        }

        var newCulture = Domain.Constants.SupportedCultures
            .FirstOrDefault(x => c.Value.Equals(x, StringComparison.OrdinalIgnoreCase));

        if (newCulture is null)
        {
            output.CultureNotFound(c.Value);
            return Maybe<User>.None;
        }

        var updatedUser = currentUser.UpdateCulture(Name.Parse(newCulture));
        return Maybe<User>.From(updatedUser);
    }
}