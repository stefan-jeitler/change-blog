using System;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;

namespace ChangeTracker.DataAccess.Postgres.Tests.TestDoubles
{
    public static class TestData
    {
        public static readonly Account Account = new(
            Guid.Parse("80f00d91-97ff-4050-8eb0-6f4484b4df99"),
            Name.Parse("TestAccount DataAccess tests"),
            Defaults.VersioningSchemeId,
            DateTime.Parse("2021-05-21"),
            null);

        public static readonly User User = new(
            Guid.Parse("9e13bbfc-451b-450e-a73a-72b6b23df64d"),
            Email.Parse("test.dataAccess@change-tracker.com"),
            Name.Parse("test"),
            Name.Parse("dataAccess"),
            Name.Parse("Europe/Berlin"),
            null,
            DateTime.Parse("2021-05-21")
        );

        public static readonly VersioningScheme DefaultScheme = new(
            Guid.Parse("4091b948-9bc5-43ee-9f98-df3d27853565"),
            Name.Parse("SemVer 2.0.0"),
            Text.Parse(
                @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$")
            ,
            Text.Parse("Semantic Versioning 2.0.0 - see https://semver.org/ for more info"),
            null,
            User.Id,
            null,
            DateTime.Parse("2021-04-05T15:00:00"));

        public static readonly Product Product = new(
            Guid.Parse("dcfe1635-1b3a-4c20-89d0-889dcbb66fed"),
            Account.Id,
            Name.Parse("TestProduct DataAccess tests"),
            DefaultScheme,
            Name.Parse("en"),
            User.Id,
            DateTime.Parse("2021-05-21"),
            null
        );
    }
}