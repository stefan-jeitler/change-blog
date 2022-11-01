using System;

namespace ChangeBlog.Api.Shared;

public static class StringExtensions
{
    public static string FirstCharToLower(this string input) =>
        string.IsNullOrWhiteSpace(input)
            ? input
            : string.Concat(input[0].ToString().ToLower(), input.AsSpan(1));
}