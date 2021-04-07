using System;
using ChangeTracker.Domain.ChangeLogVersion;
using FluentAssertions;
using Xunit;

// ReSharper disable InconsistentNaming

namespace ChangeTracker.Domain.Tests.ChangeLogVersionTests
{
    public class ClVersionTests
    {
        private const string TestVersion = "1.2.3";

        [Fact]
        public void Parse_ValidVersion_Successful()
        {
            var version = ClVersion.Parse(TestVersion);

            version.Value.Should().Be("1.2.3");
        }

        [Fact]
        public void Parse_WithNullArgument_ArgumentNullException()
        {
            Func<ClVersion> act = () => ClVersion.Parse(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Parse_WithEmptyString_ArgumentException()
        {
            Func<ClVersion> act = () => ClVersion.Parse(string.Empty);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_TooLongVersion61Characters_ArgumentException()
        {
            const string tooLongVersion = "6.1.1.1234+a6eda1a40b5261efa6496ad10f685199e93a37793a37793a37";

            Func<ClVersion> act = () => ClVersion.Parse(tooLongVersion);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_WithWhitespaceOnly_ArgumentException()
        {
            Func<ClVersion> act = () => ClVersion.Parse(" ");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_WithWhitespacesInTheMiddle_ArgumentException()
        {
            Func<ClVersion> act = () => ClVersion.Parse("1.2 .3");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData(" 1.2.3")]
        [InlineData("1.2.3 ")]
        [InlineData(" 1.2.3 ")]
        [InlineData(" 1.2.3  ")]
        public void Parse_WithLeadingAndTrailingWhitespaces_WhitespacesRemoved(string versionValue)
        {
            var version = ClVersion.Parse(versionValue);

            version.Value.Should().Be(versionValue.Trim());
        }

        [Fact]
        public void TryParse_ValidVersion_Successful()
        {
            var isSuccess = ClVersion.TryParse(TestVersion, out var version);

            isSuccess.Should().BeTrue();
            version.Value.Should().Be(TestVersion);
        }

        [Fact]
        public void TryParse_InvalidVersion_ReturnsFalse()
        {
            var isSuccess = ClVersion.TryParse(string.Empty, out var version);

            isSuccess.Should().BeFalse();
            version.Should().BeNull();
        }

        [Fact]
        public void ImplicitCast_ToString_Exists()
        {
            var version = ClVersion.Parse(TestVersion);

            string v = version;

            v.Should().Be(version.Value);
        }

        [Fact]
        public void Match_SemVer2Version_ReturnsTrue()
        {
            var version = ClVersion.Parse("1.2.3-dev.0");

            var success = version.Match(Defaults.VersioningScheme);

            success.Should().BeTrue();
        }

        [Fact]
        public void Match_NoSemVer2Version_ReturnsFalse()
        {
            var version = ClVersion.Parse("1.2.3.DEV");

            var success = version.Match(Defaults.VersioningScheme);

            success.Should().BeFalse();
        }
    }
}