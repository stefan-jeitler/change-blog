using System;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ChangeLogTests
{
    public class LabelTests
    {
        [Fact]
        public void Parse_WithValidName_Successful()
        {
            var labelName = Label.Parse("Feature");

            labelName.Value.Should().Be("Feature");
        }
        
        [Fact]
        public void Parse_WithNumber_Successful()
        {
            var labelName = Label.Parse("Feature001");

            labelName.Value.Should().Be("Feature001");
        }

        [Fact]
        public void Parse_WithEmptyString_ArgumentException()
        {
            Func<Label> act = () => Label.Parse(string.Empty);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_WithNull_ArgumentNullException()
        {
            Func<Label> act = () => Label.Parse(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Parse_WithOneCharacter_ArgumentException()
        {
            Func<Label> act = () => Label.Parse("a");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_With51Character_ArgumentException()
        {
            var tooLongLabelName = new string('a', 51);

            Func<Label> act = () => Label.Parse(tooLongLabelName);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_WithWhitespace_ArgumentException()
        {
            Func<Label> act = () => Label.Parse(" ");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_WithUnderscore_ArgumentException()
        {
            Func<Label> act = () => Label.Parse("Label_Test");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_WithDash_ArgumentException()
        {
            Func<Label> act = () => Label.Parse("Label-Test");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Parse_With50CharactersAndTrailingWhitespace_WhitespaceRemoved()
        {
            var name = new string('a', 50) + " ";

            var labelName = Label.Parse(name);

            labelName.Value.Should().Be(name.Trim());
        }

        [Fact]
        public void Parse_With50CharactersAndLeadingWhitespace_WhitespaceRemoved()
        {
            var name = " " + new string('a', 50);

            var labelName = Label.Parse(name);

            labelName.Value.Should().Be(name.Trim());
        }

        [Fact]
        public void Parse_WithWhitespaceInTheMiddle_ArgumentException()
        {
            const string name = "Bug Feature";

            Func<Label> act = () => Label.Parse(name);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void TryParse_ValidName_Successful()
        {
            const string validLabelName = "Feature";

            var isSuccess = Label.TryParse(validLabelName, out var labelName);

            isSuccess.Should().BeTrue();
            labelName.Value.Should().Be(validLabelName);
        }

        [Fact]
        public void TryParse_InvalidName_CannotBeParsed()
        {
            const string invalidLabelName = "Feature Bug";

            var isSuccess = Label.TryParse(invalidLabelName, out var labelName);

            isSuccess.Should().BeFalse();
            labelName.Should().BeNull();
        }

        [Fact]
        public void ImplicitCast_CastToString_Exists()
        {
            var labelName = Label.Parse("Feature");

            string labelStringified = labelName;

            labelStringified.Should().Be(labelName.Value);
        }
    }
}