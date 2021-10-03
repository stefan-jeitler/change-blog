using System;

namespace ChangeBlog.Domain.Miscellaneous
{
    public record Name
    {
        public const uint MinLength = 2;
        public const uint MaxLength = 50;

        private Name(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Name Parse(string candidate)
        {
            var exception = ParseInternal(candidate, out var name);

            if (exception is null)
                return name;

            throw exception;
        }

        public static bool TryParse(string candidate, out Name name)
        {
            var exception = ParseInternal(candidate, out name);

            return exception is null;
        }

        private static Exception ParseInternal(string candidate, out Name name)
        {
            name = null;

            if (candidate is null)
                return new ArgumentNullException(nameof(candidate));

            var c = candidate.Trim();

            if (c == string.Empty)
                return new ArgumentException("Name cannot be empty.");

            if (c.Length < MinLength)
                return new ArgumentException($"Name too short: min length {MinLength}");

            if (c.Length > MaxLength)
                return new ArgumentException($"Name too long: max length {MaxLength}");

            name = new Name(c);
            return null;
        }

        public static implicit operator string(Name name) => name.Value;
    }
}