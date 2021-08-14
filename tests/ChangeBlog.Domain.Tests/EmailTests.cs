using System;
using ChangeBlog.Domain.Common;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Domain.Tests
{
    public class EmailTests
    {
        [Fact]
        public void Parse_ValidEmailAddress_Successful()
        {
            var email = Email.Parse("stefan@changeBlog");

            email.Value.Should().Be("stefan@changeBlog");
        }

        [Fact]
        public void Parse_WithNullArgument_ArgumentNullException()
        {
            Func<Email> act = () => Email.Parse(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Parse_WithEmptyString_ArgumentException()
        {
            Func<Email> act = () => Email.Parse(string.Empty);

            act.Should().ThrowExactly<ArgumentException>();
        }


        [Fact]
        public void Parse_WithWhitespace_ArgumentException()
        {
            Func<Email> act = () => Email.Parse(" ");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_With255Characters_ArgumentException()
        {
            var tooLongEmail = new string('a', 250) + "@abcd";

            Func<Email> act = () => Email.Parse(tooLongEmail);

            tooLongEmail.Length.Should().Be(255);
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("stefan@changeBlog ")]
        [InlineData(" stefan@changeBlog")]
        [InlineData(" stefan@changeBlog ")]
        public void Parse_WithLeadingAndTrailingWhitespaces_WhitespacesRemoved(string e)
        {
            var email = Email.Parse(e);

            email.Value.Should().Be(e.Trim());
        }

        [Fact]
        public void Parse_WithoutAtSign_ArgumentException()
        {
            Func<Email> act = () => Email.Parse("stefanAtChangeBlog");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void TryParse_ValidEmail_Successful()
        {
            var isSuccess = Email.TryParse("stefan@changeBlog", out var email);

            isSuccess.Should().BeTrue();
            email.Value.Should().Be("stefan@changeBlog");
        }

        [Fact]
        public void TryParse_InvalidEmail_Fail()
        {
            var isSuccess = Email.TryParse("stefanAtChangeBlog", out var email);

            isSuccess.Should().BeFalse();
            email.Should().BeNull();
        }

        [Fact]
        public void ImplicitCast_CastToStringExists()
        {
            var email = Email.Parse("stefan@changeBlog");

            string e = email;

            e.Should().Be("stefan@changeBlog");
        }
    }
}
