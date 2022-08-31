using System;
using Ardalis.GuardClauses;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace ChangeBlog.Api.Shared.DTOs;

public class ApiInfo
{
    public ApiInfo(string name, string version, string environment)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Version = Guard.Against.NullOrEmpty(version, nameof(version));
        Environment = Guard.Against.NullOrEmpty(environment, nameof(environment));
    }

    public string Name { get; }
    public string Version { get; }
    public string Environment { get; }
}