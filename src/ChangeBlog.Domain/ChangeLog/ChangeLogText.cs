using System;

namespace ChangeBlog.Domain.ChangeLog
{
    public record ChangeLogText
    {
        public const ushort MaxLength = 200;
        public const ushort MinLength = 5;

        private ChangeLogText(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public virtual bool Equals(ChangeLogText other)
        {
            if (other is null)
                return false;

            return ReferenceEquals(this, other) ||
                   string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public static ChangeLogText Parse(string candidate)
        {
            var exception = ParseInternal(candidate, out var text);

            if (exception is null)
                return text;

            throw exception;
        }

        public static bool TryParse(string candidate, out ChangeLogText text)
        {
            var exception = ParseInternal(candidate, out text);

            return exception is null;
        }

        private static Exception ParseInternal(string candidate, out ChangeLogText text)
        {
            text = null;

            if (candidate is null)
                return new ArgumentNullException(nameof(candidate));

            var c = candidate.Trim();

            if (c == string.Empty)
                return new ArgumentException("Text must not be empty.");

            switch (c.Length)
            {
                case > MaxLength:
                    return new ArgumentException($"The text must not contain more than {MaxLength} characters.");
                case < MinLength: return new ArgumentException($"A Label needs at least {MinLength} characters.");
                default:
                    text = new ChangeLogText(c);
                    return null;
            }
        }

        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

        public static implicit operator string(ChangeLogText text) => text.Value;
    }
}