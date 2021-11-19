using System;

namespace ChangeBlog.Domain;

public record ProductUserRole
{
    public ProductUserRole(Guid userId, Guid productId, Guid roleId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.");

        UserId = userId;

        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty.");

        ProductId = productId;

        if (roleId == Guid.Empty)
            throw new ArgumentException("RoleId cannot be empty.");

        RoleId = roleId;
    }

    public Guid UserId { get; }
    public Guid ProductId { get; }
    public Guid RoleId { get; }
}