using System;
using NodaTime;

namespace ChangeBlog.Application.Extensions;

public static class DateTimeExtensions
{
    public static DateTimeOffset ToLocal(this DateTime dateTime, string olsonId)
    {
        ArgumentNullException.ThrowIfNull(olsonId);

        var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(olsonId);
        if (timeZone is null)
        {
            throw new Exception($"TimeZone not found: {olsonId}");
        }

        var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));

        return instant.InZone(timeZone).ToDateTimeOffset();
    }

    public static DateTime ToUtc(this DateTime dateTime, string olsonId)
    {
        ArgumentNullException.ThrowIfNull(olsonId);
        
        var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(olsonId);
        if (timeZone is null)
        {
            throw new Exception($"TimeZone not found: {olsonId}");
        }

        var localDateTime = LocalDateTime.FromDateTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Local));

        return localDateTime
            .InZoneStrictly(timeZone)
            .WithZone(DateTimeZone.Utc)
            .ToDateTimeUtc();
    }
}