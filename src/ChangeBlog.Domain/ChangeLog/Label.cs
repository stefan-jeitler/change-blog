using System;
using System.Text.RegularExpressions;

namespace ChangeBlog.Domain.ChangeLog
{
    public record Label
    {
        public const ushort MinLength = 2;
        public const ushort MaxLength = 50;

        private Label(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Label Parse(string candidate)
        {
            var exception = ParseInternal(candidate, out var labelName);

            if (exception is null)
                return labelName;

            throw exception;
        }

        public static bool TryParse(string candidate, out Label label)
        {
            var exception = ParseInternal(candidate, out label);

            return exception is null;
        }

        private static Exception ParseInternal(string candidate, out Label label)
        {
            label = null;

            if (candidate is null)
                return new ArgumentNullException(nameof(candidate), "Label must not be null.");

            var c = candidate.Trim();

            if (c.Contains(' '))
                return new ArgumentException("Use single words only.");

            if (c == string.Empty)
                return new ArgumentException("Label must not be empty.");

            if (!Regex.IsMatch(c, @"^[a-zA-Z0-9]+$"))
                return new ArgumentException("Label contains invalid characters. Only letters and numbers allowed.");

            switch (c.Length)
            {
                case < MinLength: return new ArgumentException("A Label needs at least one character.");
                case > MaxLength: return new ArgumentException("A label must not contain more than 50 characters.");
                default:
                    label = new Label(c);
                    return null;
            }
        }

        public static implicit operator string(Label label) => label.Value;
    }
}