using System;
using System.Text.RegularExpressions;

namespace ChangeBlog.Domain.Version;

public record ClVersionValue
{
    private const int MaxLength = 60;

    private ClVersionValue(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ClVersionValue Parse(string candidate)
    {
        var exception = ParseInternal(candidate, out var version);

        if (exception is null)
            return version;

        throw exception;
    }

    public static bool TryParse(string candidate, out ClVersionValue versionValue)
    {
        var exception = ParseInternal(candidate, out versionValue);

        return exception is null;
    }

    public bool Match(VersioningScheme scheme)
    {
        var regex = new Regex(scheme.RegexPattern.Value);

        return regex.Match(Value).Success;
    }

    private static Exception ParseInternal(string candidate, out ClVersionValue versionValue)
    {
        versionValue = null;

        if (candidate is null)
            return new ArgumentNullException(nameof(candidate));

        var c = candidate.Trim();

        if (c == string.Empty)
            return new ArgumentException("Version value cannot be empty.", nameof(candidate));

        if (c.Contains(" "))
            return new ArgumentException("Whitespaces in a versionValue are not allowed.");

        if (c.Length > MaxLength)
            return new ArgumentException($"Version too long. max length {MaxLength}.");

        versionValue = new ClVersionValue(c);
        return null;
    }

    public static implicit operator string(ClVersionValue versionValue)
    {
        return versionValue.Value;
    }
}