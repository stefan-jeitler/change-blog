using System;
using System.Collections.Generic;
using System.Linq;

namespace ChangeTracker.Domain.Authorization
{
    public class AuthorizationService
    {
        private readonly IList<Role> _accountRoles;
        private readonly IList<Role> _productRoles;

        public AuthorizationService(IEnumerable<Role> accountRoles)
            : this(accountRoles, Enumerable.Empty<Role>())
        {
        }

        public AuthorizationService(IEnumerable<Role> accountRoles,
            IEnumerable<Role> productRoles)
        {
            if (accountRoles is null)
                throw new ArgumentNullException(nameof(accountRoles));

            _accountRoles = accountRoles.ToList();

            if (productRoles is null)
                throw new ArgumentNullException(nameof(productRoles));

            _productRoles = productRoles.ToList();
        }

        public AuthorizationState GetAuthorizationState(Permission permission)
        {
            if (_accountRoles.All(x => x.Name.Value != Role.DefaultUser))
            {
                return AuthorizationState.Inaccessible;
            }

            if (_productRoles.Count > 0)
            {
                return _productRoles.Any(r => r.Permissions.Contains(permission))
                    ? AuthorizationState.Authorized
                    : AuthorizationState.Unauthorized;
            }

            return _accountRoles.Any(r => r.Permissions.Contains(permission))
                ? AuthorizationState.Authorized
                : AuthorizationState.Unauthorized;
        }
    }
}