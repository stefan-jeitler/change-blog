using System;
using ChangeBlog.Domain.Authorization;

namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users;

public class RolePermissionDto
{
    public RolePermissionDto(string type, Guid id, string name, string description, string permission,
        DateTime createdAt)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));

        if (Enum.TryParse<Permission>(permission, true, out var p))
        {
            Permission = p;
        }

        CreatedAt = createdAt;
    }

    public string Type { get; }
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public Permission? Permission { get; }
    public DateTime CreatedAt { get; }
}