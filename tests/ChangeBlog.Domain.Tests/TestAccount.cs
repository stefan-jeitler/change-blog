using System;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;

namespace ChangeBlog.Domain.Tests;

public static class TestAccount
{
    public static readonly Guid Id = Guid.Parse("c1b588ee-069d-453e-8f74-fc43b7ae0649");
    public static readonly Name Name = Name.Parse("Test Account");
    public static readonly Guid UserId = Guid.Parse("a1b89f2d-d13f-4572-8522-8a92fb4fdb6a");
    public static readonly DateTime CreationDate = DateTime.Parse("2021-04-06");

    /// <summary>
    ///     Not valid: *.12 | *123.1 | 12* | 12.*.34
    ///     Valid: 1.23.456 | 1.23 | 1.23.* | 1.* | 1
    /// </summary>
    public static readonly VersioningScheme CustomVersioningScheme = new(
        Guid.Parse("76a96500-6446-42b3-bb3d-5e318b338b0d"),
        Name.Parse("My custom scheme"),
        Text.Parse(@"^(\d+\.)?(\d+\.)?(\*|\d+)$"),
        Text.Parse("a custom scheme for my product"),
        Id,
        UserId,
        null,
        DateTime.Parse("2021-04-05T15:00:00"));

    public static readonly VersioningScheme DefaultVersioningScheme = new(
        Guid.Parse("4091b948-9bc5-43ee-9f98-df3d27853565"),
        Name.Parse("SemVer 2.0.0"),
        Text.Parse(
            @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$")
        ,
        Text.Parse("Semantic Versioning 2.0.0 - see https://semver.org/ for more info"),
        null,
        UserId,
        null,
        DateTime.Parse("2021-04-05T15:00:00"));
}