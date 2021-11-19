using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace ChangeBlog.Api.Shared.DTOs;

public class ApiInfo
{
    public ApiInfo(string name, string version, string environment)
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name));

        if (name == string.Empty)
            throw new ArgumentException("name cannot be empty.");

        Name = name;

        if (version is null)
            throw new ArgumentNullException(nameof(version));

        if (version == string.Empty)
            throw new ArgumentException("version cannot be empty");

        Version = version;

        Environment = environment switch
        {
            null => throw new ArgumentNullException(nameof(environment)),
            "" => throw new ArgumentException("environment cannot be empty"),
            _ => environment
        };
    }

    public string Name { get; }
    public string Version { get; }
    public string Environment { get; }
}