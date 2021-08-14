using System;
using ChangeBlog.Domain.Common;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Domain.Tests
{
    public class TextTests
    {
        [Fact]
        public void Parse_ValidText_Successful()
        {
            var text = Text.Parse("some text");

            text.Value.Should().Be("some text");
        }

        [Fact]
        public void Parse_NullText_ArgumentNullException()
        {
            Func<Text> act = () => Text.Parse(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Parse_EmptyText_ArgumentException()
        {
            Func<Text> act = () => Text.Parse(string.Empty);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_WithTooLongText501Characters_ArgumentException()
        {
            var tooLong = new string('a', 501);

            Func<Text> act = () => Text.Parse(tooLong);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData(" some test")]
        [InlineData(" some test ")]
        [InlineData("some test ")]
        [InlineData("  some test")]
        public void Parse_WithLeadingAndTrailingWhitespaces_WhitespacesRemoved(string value)
        {
            var text = Text.Parse(value);

            text.Value.Should().Be(value.Trim());
        }

        [Fact]
        public void TryParse_ValidText_Successful()
        {
            var isSuccess = Text.TryParse("some text", out var text);

            isSuccess.Should().BeTrue();
            text.Value.Should().Be("some text");
        }

        [Fact]
        public void ImplicitCast_CastToString_Exists()
        {
            var t = Text.Parse("some text");

            string text = t;

            text.Should().Be(t.Value);
        }
    }
}
