using System;

namespace ChangeTracker.Domain.ChangeLog
{
    public record Issue
    {
        private const int MaxLength = 50;

        private Issue(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Issue Parse(string candidate)
        {
            var exception = ParseInternal(candidate, out var issue);

            if (exception is null)
                return issue;

            throw exception;
        }

        public static bool TryParse(string candidate, out Issue issue)
        {
            var exception = ParseInternal(candidate, out issue);

            return exception is null;
        }

        private static Exception ParseInternal(string candidate, out Issue issue)
        {
            issue = null;

            if (candidate is null)
                return new ArgumentNullException(nameof(candidate));

            var c = candidate.Trim();

            if (c == string.Empty)
                return new ArgumentException("Issue number cannot be empty.");

            if (c.Contains(' '))
                return new ArgumentException("Whitespaces not allowed.");

            if (c.Length > MaxLength)
                return new ArgumentException($"Issue number cannot have more than {MaxLength} characters.");

            issue = new Issue(c);
            return null;
        }
    }
}