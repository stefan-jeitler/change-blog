using System;
using ChangeBlog.Application.Extensions;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.ExtensionsTests
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void ToLocal_ValidTimeZone_ReturnLocalTime()
        {
            var utc = DateTime.Parse("2021-05-14T12:00:00");
            const string timeZone = "Europe/Berlin";

            var local = utc.ToLocal(timeZone);

            var expected = DateTimeOffset.Parse("2021-05-14T14:00:00+02:00");
            local.Should().Be(expected);
        }

        [Fact]
        public void ToLocal_NullTimeZone_ArgumentException()
        {
            var utc = DateTime.Parse("2021-05-14T12:00:00Z");

            Func<DateTimeOffset> act = () => utc.ToLocal(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
