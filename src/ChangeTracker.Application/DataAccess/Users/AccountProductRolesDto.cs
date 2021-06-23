using System;
using System.Collections.Generic;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Authorization;

namespace ChangeTracker.Application.DataAccess.Users
{
    public class AccountProductRolesDto
    {
        public AccountProductRolesDto(IEnumerable<Role> accountRoles,
            IEnumerable<Role> productRoles)
        {
            AccountPermissions = accountRoles ?? throw new ArgumentNullException(nameof(accountRoles));
            ProductPermissions = productRoles ?? throw new ArgumentNullException(nameof(productRoles));
        }

        public IEnumerable<Role> AccountPermissions { get; }
        public IEnumerable<Role> ProductPermissions { get; }

        public void Deconstruct(out IEnumerable<Role> accountRole, 
            out IEnumerable<Role> productRole)
        {
            (accountRole, productRole) = (AccountPermissions, ProductPermissions);
        }
    }
}