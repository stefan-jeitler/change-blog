using System;

namespace ChangeBlog.Domain.Miscellaneous
{
    public record Text
    {
        public const uint MaxLength = 500;

        private Text(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Text Parse(string candidate)
        {
            var exception = ParseInternal(candidate, out var text);

            if (exception is null)
                return text;

            throw exception;
        }

        public static bool TryParse(string candidate, out Text text)
        {
            var exception = ParseInternal(candidate, out text);

            return exception is null;
        }

        private static Exception ParseInternal(string candidate, out Text text)
        {
            text = null;

            if (candidate is null)
                return new ArgumentNullException(nameof(candidate));

            var c = candidate.Trim();

            if (c == string.Empty)
                return new ArgumentException("Text cannot be empty.");

            if (c.Length > MaxLength)
                return new ArgumentException($"Too long text. max length {MaxLength}");

            text = new Text(c);
            return null;
        }

        public static implicit operator string(Text text) => text.Value;
    }
}