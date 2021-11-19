using System;
using ChangeBlog.Domain.Miscellaneous;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Domain.Tests;

public class NameTests
{
    [Fact]
    public void Parse_WithValidName_Successful()
    {
        var name = Name.Parse("e.g. product name");

        name.Value.Should().Be("e.g. product name");
    }

    [Fact]
    public void Parse_WithNullArgument_ArgumentNullException()
    {
        Func<Name> act = () => Name.Parse(null);

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Parse_WithEmptyString_ArgumentException()
    {
        Func<Name> act = () => Name.Parse(string.Empty);

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Parse_WithOneCharacter_TooShortArgumentException()
    {
        Func<Name> act = () => Name.Parse("a");

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Parse_With51Characters_TooLongArgumentException()
    {
        var name = new string('a', 51);

        Func<Name> act = () => Name.Parse(name);

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Theory]
    [InlineData(" stefan")]
    [InlineData("stefan ")]
    [InlineData(" stefan ")]
    [InlineData(" stefan  ")]
    [InlineData("  stefan")]
    public void Parse_WithTrailingAndLeadingWhitespaces_WhitespacesRemoved(string n)
    {
        var name = Name.Parse(n);

        name.Value.Should().Be(n.Trim());
    }

    [Fact]
    public void TryParse_ValidName_Successful()
    {
        var isSuccess = Name.TryParse("stefan", out var name);

        isSuccess.Should().BeTrue();
        name.Value.Should().Be("stefan");
    }

    [Fact]
    public void TryParse_InvalidName_ReturnsFalse()
    {
        var isSuccess = Name.TryParse(" ", out var name);

        isSuccess.Should().BeFalse();
        name.Should().BeNull();
    }

    [Fact]
    public void ImplicitCast_CastToString_Exists()
    {
        var name = Name.Parse("stefan");

        string n = name;

        n.Should().Be("stefan");
    }
}