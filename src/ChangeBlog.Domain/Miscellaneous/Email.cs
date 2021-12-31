using System;

namespace ChangeBlog.Domain.Miscellaneous;

public record Email
{
    private const int MaxLength = 254;

    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Email Parse(string candidate)
    {
        var exception = ParseInternal(candidate, out var email);

        if (exception is null)
        {
            return email;
        }

        throw exception;
    }

    public static bool TryParse(string candidate, out Email email)
    {
        var exception = ParseInternal(candidate, out email);

        return exception is null;
    }

    private static Exception ParseInternal(string candidate, out Email email)
    {
        email = null;

        if (candidate is null)
        {
            return new ArgumentNullException(nameof(candidate));
        }

        var c = candidate.Trim();

        if (c == string.Empty)
        {
            return new ArgumentException("Email cannot be empty.");
        }

        if (c.Length > MaxLength)
        {
            return new ArgumentException($"Email is too long. max length: {MaxLength}");
        }

        if (!c.Contains('@'))
        {
            return new ArgumentException("An email address must contain an @ sign.");
        }

        email = new Email(c);
        return null;
    }

    public static implicit operator string(Email email)
    {
        return email.Value;
    }
}