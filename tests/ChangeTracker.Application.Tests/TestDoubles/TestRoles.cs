using System;
using System.Collections.Generic;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public static class TestRoles
    {
        public static readonly IList<Role> Roles = new List<Role>
        {
            new(Guid.Parse("8ec8ae99-83d7-4958-9df5-72eb8eaf002b"), Name.Parse("DefaultUser"), Text.Parse("Basic User"),
                DateTime.Parse("2021-05-21"), new[] {Name.Parse("ViewOwnUser"), Name.Parse("ViewAccount")}),
            new(Guid.Parse("ad7b83ed-8fce-4341-978b-8d1eae66f346"), Name.Parse("Developer"), Text.Parse("Developer"),
                DateTime.Parse("2021-05-21"), new[]
                {
                    Name.Parse("ViewOwnUser"), Name.Parse("ViewAccount"), Name.Parse("AddProduct"),
                    Name.Parse("ViewChangeLogLines")
                }),
            new(Guid.Parse("446bceb2-0b9d-4899-934d-51be0576b7fa"), Name.Parse("PlatformManager"),
                Text.Parse("Platform Manager"),
                DateTime.Parse("2021-05-21"), new[]
                {
                    Name.Parse("ViewOwnUser"), Name.Parse("ViewAccount"), Name.Parse("AddProduct"),
                    Name.Parse("ViewChangeLogLines"), Name.Parse("CloseProduct"), Name.Parse("ViewUserProducts")
                })
        };
    }
}