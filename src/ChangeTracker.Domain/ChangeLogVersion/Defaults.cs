using System;
using ChangeTracker.Domain.Common;

namespace ChangeTracker.Domain.ChangeLogVersion
{
    public static class Defaults
    {
        public static readonly VersioningScheme VersioningScheme = new(
            Guid.Parse("4091b948-9bc5-43ee-9f98-df3d27853565"),
            Name.Parse("SemVer 2.0.0"),
            Text.Parse(
                @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$"),
            null,
            Text.Parse("Semantic Versioning 2.0.0 - see https://semver.org/ for more info"),
            DateTime.Parse("2021-04-05T15:00:00"));
    }
}