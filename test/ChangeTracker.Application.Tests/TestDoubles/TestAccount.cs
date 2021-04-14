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
            Text.Parse("a custom scheme for my project"),
            DateTime.Parse("2021-04-05T15:00:00"),
            null);

        public static readonly Project Project = new(Id,
            Name.Parse("Test Project"),
            CustomVersioningScheme,
            DateTime.Parse("2021-04-04"));
    }
}