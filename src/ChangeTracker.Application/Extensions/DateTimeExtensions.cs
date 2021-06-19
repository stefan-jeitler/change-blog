using System;
using NodaTime;

namespace ChangeTracker.Application.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTimeOffset ToLocal(this DateTime dateTime, string olsonId)
        {
            if (olsonId is null)
                throw new ArgumentNullException(nameof(olsonId));

            var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(olsonId);

            if (timeZone is null)
                throw new Exception($"TimeZone not found: {olsonId}");

            var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));

            return instant.InZone(timeZone).ToDateTimeOffset();
        }
    }
}