using System;
using System.Text.RegularExpressions;

namespace ChangeTracker.Domain.ChangeLogVersion
{
    public record ClVersion
    {
        private const int MaxLength = 60;

        private ClVersion(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static ClVersion Parse(string candidate)
        {
            var exception = ParseInternal(candidate, out var version);

            if (exception is null)
                return version;

            throw exception;
        }

        public static bool TryParse(string candidate, out ClVersion version)
        {
            var exception = ParseInternal(candidate, out version);

            return exception is null;
        }

        public bool Match(VersioningScheme scheme)
        {
            var regex = new Regex(scheme.RegexPattern.Value);

            return regex.Match(Value).Success;
        }

        private static Exception ParseInternal(string candidate, out ClVersion version)
        {
            version = null;

            if (candidate is null)
                return new ArgumentNullException(nameof(candidate));

            var c = candidate.Trim();

            if (c == string.Empty)
                return new ArgumentException("Version value cannot be empty.", nameof(candidate));

            if (c.Contains(" "))
                return new ArgumentException("Whitespaces in a version are not allowed.");

            if (c.Length > MaxLength)
                return new ArgumentException("Version too long; max: 60");

            version = new ClVersion(c);
            return null;
        }

        public static implicit operator string(ClVersion version) => version.Value;
    }
}