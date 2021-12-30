using System;
using System.Collections.Generic;
using ChangeBlog.Domain;

namespace ChangeBlog.Application.Boundaries.DataAccess.Users;

public class AccountProductRolesDto
{
    public AccountProductRolesDto(IEnumerable<Role> accountRoles,
        IEnumerable<Role> productRoles)
    {
        AccountRoles = accountRoles ?? throw new ArgumentNullException(nameof(accountRoles));
        ProductRoles = productRoles ?? throw new ArgumentNullException(nameof(productRoles));
    }

    public IEnumerable<Role> AccountRoles { get; }
    public IEnumerable<Role> ProductRoles { get; }

    public void Deconstruct(out IEnumerable<Role> accountRoles,
        out IEnumerable<Role> productRoles)
    {
        (accountRoles, productRoles) = (AccountRoles, ProductRoles);
    }
}