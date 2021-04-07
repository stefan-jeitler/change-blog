﻿using System;
using ChangeTracker.Domain.ChangeLogVersion;
using ChangeTracker.Domain.Common;

namespace ChangeTracker.Domain.Tests.TestDoubles
{
    public class TestAccount
    {
        public static readonly Guid Id = Guid.Parse("c1b588ee-069d-453e-8f74-fc43b7ae0649");
        public static readonly Name Name = Name.Parse("Test Account");
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
            DateTime.Parse("2021-04-05T15:00:00"));
    }
}