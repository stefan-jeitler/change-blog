using System;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;

// ReSharper disable InconsistentNaming

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public static class TestAccount
    {
        public static readonly Guid Id = Guid.Parse("c1b588ee-069d-453e-8f74-fc43b7ae0649");
        public static readonly Name Name = Name.Parse("Name");
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
            Id,
            Text.Parse("a custom scheme for my product"),
            DateTime.Parse("2021-04-05T15:00:00"),
            null);

        public static readonly VersioningScheme DefaultScheme = new(
            Guid.Parse("4091b948-9bc5-43ee-9f98-df3d27853565"),
            Name.Parse("SemVer 2.0.0"),
            Text.Parse(
                @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$")
            ,
            null,
            Text.Parse("Semantic Versioning 2.0.0 - see https://semver.org/ for more info"),
            DateTime.Parse("2021-04-05T15:00:00"),
            null);

        public static readonly Account Account = new(Id, Name, CustomVersioningScheme.Id, CreationDate, null);

        public static readonly Product Product = new(Guid.Parse("ab9f36eb-0cc5-4855-bddb-a3d5d8ad0117"),
            Id,
            Name.Parse("Test Product"),
            CustomVersioningScheme,
            UserId,
            DateTime.Parse("2021-04-04"),
            null);

        public static readonly User User = new(UserId,
            Email.Parse("test@change-tracker.com"),
            Name.Parse("test"),
            Name.Parse("user"),
            Text.Parse("Europe/Berlin"),
            null, DateTime.Parse("2021-05-16"));
    }
}