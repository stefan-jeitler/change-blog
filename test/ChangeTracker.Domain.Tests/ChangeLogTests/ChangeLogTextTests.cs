﻿using System;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ChangeLogTests
{
    public class ChangeLogTextTests
    {
        [Fact]
        public void Parse_WithValidText_Successful()
        {
            const string changeLogText = "New feature added";

            var text = ChangeLogText.Parse(changeLogText);

            text.Value.Should().Be(changeLogText);
        }

        [Fact]
        public void Parse_WithNullArgument_ArgumentNullException()
        {
            Func<ChangeLogText> act = () => ChangeLogText.Parse(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Parse_WithEmptyString_ArgumentException()
        {
            Func<ChangeLogText> act = () => ChangeLogText.Parse(string.Empty);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_WithWhitespaceOnly_ArgumentException()
        {
            Func<ChangeLogText> act = () => ChangeLogText.Parse(" ");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData(" New feature added")]
        [InlineData("New feature added ")]
        [InlineData(" New feature added ")]
        [InlineData("  New feature added")]
        [InlineData("New feature added  ")]
        public void Parse_WithWhitespaces_WhitespacesRemoved(string changeLogLine)
        {
            var text = ChangeLogText.Parse(changeLogLine);

            text.Value.Should().Be(changeLogLine.Trim());
        }

        [Fact]
        public void Parse_With201Characters_ArgumentException()
        {
            var tooLongText = new string('a', 201);

            Func<ChangeLogText> act = () => ChangeLogText.Parse(tooLongText);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_With4Characters_ArgumentException()
        {
            const string tooShortText = "Test";

            Func<ChangeLogText> act = () => ChangeLogText.Parse(tooShortText);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("Tests")]
        [InlineData(
            "200 Characters 200 Characters 200 Characters 200 Characters 200 Characters 200 Characters 200 Characters 200 Characters 200 Characters 200 Characters 200 Characters 200 Characters 200 Characters 200 2")]
        public void Parse_WithMinAndMaxLength_Successful(string text)
        {
            var changeLogText = ChangeLogText.Parse(text);

            changeLogText.Value.Length.Should().BeInRange(5, 200);
        }

        [Fact]
        public void TryParse_ValidText_Successful()
        {
            const string changeLogText = "New feature changeLog";
            var isSuccess = ChangeLogText.TryParse(changeLogText, out var text);

            isSuccess.Should().BeTrue();
            text.Value.Should().Be(changeLogText);
        }

        [Fact]
        public void TryParse_InvalidText_Fail()
        {
            const string changeLogText = " ";
            var isSuccess = ChangeLogText.TryParse(changeLogText, out var text);

            isSuccess.Should().BeFalse();
            text.Should().BeNull();
        }

        [Fact]
        public void ImplicitCast_CastToString_Exists()
        {
            var text = ChangeLogText.Parse("New Feature added");

            string labelStringified = text;

            labelStringified.Should().Be(text.Value);
        }
    }
}